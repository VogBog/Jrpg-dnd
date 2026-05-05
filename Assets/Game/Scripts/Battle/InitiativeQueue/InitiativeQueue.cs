using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.DamageDealing.Health.Events;
using Game.Scripts.Events.Data;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Interfaces;
using Game.Scripts.Scenes;
using Zenject;

namespace Game.Scripts.Battle.InitiativeQueue
{
    public class InitiativeQueue : IInitiativeQueue, IDisposable
    {
        private readonly List<InitiativeUnitData> _units = new(8);
        private int _turnIndex = 0;

        private ICheckRoller _checkRoller;
        private IEventBus _bus;
        private IEventPool _pool;
        private IAsyncOperationScope _scope;
        
        public InitiativeUnitData UnitMakingTurn => _units.Count > 0 ? _units[_turnIndex] : default;
        public IEnumerable<InitiativeUnitData> Queue => _units;
        
        public event Action<InitiativeQueueChangedEvent> UnitAdded; 
        public event Action<InitiativeQueueChangedEvent> UnitRemoved;
        public event Action<InitiativeUnitData> NextTurnStarted; 

        [Inject]
        public void Construct(
            ICheckRoller checkRoller,
            IEventBus eventBus,
            IEventPool eventPool,
            IAsyncOperationScope scope)
        {
            _checkRoller = checkRoller;
            _bus = eventBus;
            _pool = eventPool;
            _scope = scope;

            _bus.Subscribe<DiedEvent>(OnUnitDied).On(EventStep.SeeResults);
        }

        public void Dispose()
        {
            if (_bus != null)
                _bus.Unsubscribe<DiedEvent>(OnUnitDied);
        }

        public async UniTask AddUnit(IBattleUnit unit, CancellationToken ct)
        {
            if (_units.FirstOrDefault(x => x.Unit == unit).Unit != null)
                throw new InvalidOperationException("InitiativeQueue.AddUnit: This unit is already added to this queue.");

            var initiativeResult = await _checkRoller.InitiativeCheck(unit, ct);
            int initiative = initiativeResult.Result;
            ct.ThrowIfCancellationRequested();
            
            var data = new InitiativeUnitData(unit, initiative);

            for (int i = 0; i < _units.Count; i++)
            {
                if (_units[i].Initiative < initiative)
                {
                    if (_turnIndex >= i)
                        _turnIndex++;
                    
                    _units.Insert(i, data);
                    UnitAdded?.Invoke(new (_units, data, i));
                    return;
                }
            }
            
            _units.Add(data);
            UnitAdded?.Invoke(new(_units, data, _units.Count - 1));
        }

        public void AddUnitInTheEnd(IBattleUnit unit)
        {
            if (_units.FirstOrDefault(x => x.Unit == unit).Unit != null)
                throw new InvalidOperationException("InitiativeQueue.AddUnit: This unit is already added to this queue.");
            
            var data = new InitiativeUnitData(unit, 0);
            _units.Add(data);
            UnitAdded?.Invoke(new(_units, data, _units.Count - 1));
        }

        public async UniTask RemoveUnitAsync(IBattleUnit unit, CancellationToken ct)
        {
            int index = _units.FindIndex(x => x.Unit == unit);
            if (index == -1)
                return;
            
            if (index == _turnIndex)
                await EndTurnAsync(ct);
            
            var data = _units[index];
            _units.RemoveAt(index);
            if (_turnIndex >= index)
                _turnIndex--;
            
            UnitRemoved?.Invoke(new(_units, data, index));
        }

        public void EndTurn()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(EndTurnAsync(ct));
        }

        public void StartFirstTurn()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(StartFirstTurnAsync(ct));
        }

        private async UniTask StartFirstTurnAsync(CancellationToken ct)
        {
            await UniTask.NextFrame();
            ct.ThrowIfCancellationRequested();
            
            _turnIndex = 0;
            if (_units.Count == 0)
                return;

            var ev = _pool.Get<TurnStartingEvent>().Init(_units[0], true);
            await _bus.Publish(ev, ct);
            _pool.Return(ev);
            
            await _units[0].Unit.OnStartTurnAsync(ct);
            NextTurnStarted?.Invoke(_units[0]);
        }
        
        private async UniTask EndTurnAsync(CancellationToken ct)
        {
            await UniTask.NextFrame();
            ct.ThrowIfCancellationRequested();
            
            var endEv = _pool.Get<TurnEndingEvent>().Init(_units[_turnIndex]);
            await _bus.Publish(endEv, ct);
            _pool.Return(endEv);
            
            await _units[_turnIndex].Unit.OnEndTurnAsync(ct);
            
            _turnIndex = (_turnIndex + 1) % _units.Count;
            
            var startEv = _pool.Get<TurnStartingEvent>().Init(_units[_turnIndex], false);
            await _bus.Publish(startEv, ct);
            _pool.Return(startEv);
            
            await _units[_turnIndex].Unit.OnStartTurnAsync(ct);

            NextTurnStarted?.Invoke(_units[_turnIndex]);
        }

        private async UniTask OnUnitDied(DiedEvent ev, CancellationToken ct)
        {
            await RemoveUnitAsync(ev.Unit, ct);
        }
    }
}