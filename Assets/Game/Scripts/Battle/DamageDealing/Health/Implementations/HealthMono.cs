using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Battle.DamageDealing.Health.Events;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Data;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.DamageDealing.Health.Implementations
{
    public class HealthMono : MonoBehaviour, IHealthProcessor
    {
        private IEventPool _eventPool;
        private IBattleUnit _battleUnit;
        private IEventBus _eventBus;

        private Dictionary<DamageType, DamageReactionTypes> _damageReactions;
        private bool _died = false;
        
        [field: SerializeField] public int Value { get; private set; }
        
        public int MaxValue { get; private set; }
        
        public event Action<IHealthProcessor> Changed;

        [Inject]
        private void InjectAll(IEventPool eventPool, IBattleUnit battleUnit, IEventBus eventBus)
        {
            _battleUnit = battleUnit;
            _eventPool = eventPool;
            _eventBus = eventBus;
            MaxValue = Value;
        }

        public IEnumerable<(DamageType, DamageReactionTypes)> GetDamageReactions() 
            => _damageReactions?.Select(kvp => (kvp.Key, kvp.Value)) ?? Array.Empty<(DamageType, DamageReactionTypes)>();

        public async UniTask DealDamage(DealDamageCommand command, CancellationToken ct)
        {
            if (command.DamageList.Values == null || command.DamageList.Values.Count == 0 || _died)
                return;

            if (EventServicesInitialized())
            {
                var (cancel, cmd) = await InvokeTakingDamageEvent(command, ct);
                if (cancel)
                    return;
                command = cmd;
            }

            
            int damage = CalculateDamage(command.DamageList);
            if (command.IsCritical)
                damage *= 2;
            
            Value = Mathf.Clamp(Value - damage, 0, MaxValue);
            Changed?.Invoke(this);
            

            if (EventServicesInitialized())
            {
                await InvokeTakenDamageEvent(command, damage, ct);
            }

            if (Value == 0)
                await Die(command, ct);
        }

        public async UniTask Heal(HealCommand command, CancellationToken ct)
        {
            if (command.Heal < 0)
                throw new ArgumentException($"HealthMono.Heal({command.Heal}): heal must be equals or greater than 0.");

            if (EventServicesInitialized())
            {
                var (cancel, cmd) = await InvokeHealingEvent(command, ct);
                if (cancel)
                    return;
                command = cmd;
            }
            
            
            Value = Mathf.Clamp(Value + command.Heal, 0, MaxValue);
            Changed?.Invoke(this);
        }

        public void Set(int value)
        {
            Value = Mathf.Clamp(value, 0, MaxValue);
            Changed?.Invoke(this);
        }

        public void SetDamageReaction(DamageType type, DamageReactionTypes reaction)
        {
            if (_damageReactions == null && reaction is DamageReactionTypes.Default)
                return;

            _damageReactions ??= new();
            if (_damageReactions.ContainsKey(type))
            {
                if (reaction is not DamageReactionTypes.Default)
                    _damageReactions[type] = reaction;
                else
                    _damageReactions.Remove(type);
                return;
            }

            if (reaction is DamageReactionTypes.Default)
                return;
            
            _damageReactions.Add(type, reaction);
        }

        private async UniTask Die(DealDamageCommand command, CancellationToken ct)
        {
            var dyingEv = _eventPool.Get<DyingEvent>().Init(_battleUnit, command);
            await _eventBus.Publish(dyingEv, ct);
            _eventPool.Return(dyingEv);
            ct.ThrowIfCancellationRequested();

            if (Value > 0)
                return;

            _died = true;
            var diedEv = _eventPool.Get<DiedEvent>().Init(_battleUnit);
            await _eventBus.Publish(diedEv, ct);
            _eventPool.Return(diedEv);
            
            ct.ThrowIfCancellationRequested();
        }

        private int CalculateDamage(RolledDamageList damageList)
        {
            int result = 0;
            foreach (var damageValue in damageList.Values)
            {
                int damage = damageValue.Value;
                if (_damageReactions != null &&
                    _damageReactions.TryGetValue(damageValue.DamageType, out var reaction))
                {
                    damage = reaction switch
                    {
                        DamageReactionTypes.Default => damage,
                        DamageReactionTypes.Resist => damage / 2,
                        DamageReactionTypes.Immune => 0,
                        DamageReactionTypes.Weakness => damage + damage,
                        _ => throw new InvalidEnumArgumentException(
                            $"HealthMono.CalculateDamage: Has no reaction for {reaction}")
                    };
                }

                result += damage;
            }

            if (result < 0)
                result = 0;

            return result;
        }
        
        private bool EventServicesInitialized() => _battleUnit != null && _eventPool != null && _eventBus != null;

        private async UniTask<(bool, DealDamageCommand)> InvokeTakingDamageEvent(
            DealDamageCommand command,
            CancellationToken ct)
        {
            var takingDamageEvent = _eventPool.Get<TakingDamageEvent>().SetData(this, _battleUnit, command);
            
            await _eventBus.Publish(takingDamageEvent, ct);
            
            command = takingDamageEvent.Command;
            bool cancel = takingDamageEvent.IsCancelled;
            _eventPool.Return(takingDamageEvent);

            return (cancel, command);
        }

        private async UniTask InvokeTakenDamageEvent(
            DealDamageCommand command,
            int damage,
            CancellationToken ct)
        {
            var ev = _eventPool.Get<TakenDamageEvent>()
                .SetData(this, _battleUnit, command, damage);
            await _eventBus.Publish(ev, ct);
            
            _eventPool.Return(ev);
        }
        
        private async UniTask<(bool, HealCommand)> InvokeHealingEvent(
            HealCommand command,
            CancellationToken ct)
        {
            var healEvent = _eventPool.Get<HealingEvent>().SetData(this, _battleUnit, command);
            await _eventBus.Publish(healEvent, ct);
            
            bool cancel = healEvent.IsCancelled;
            command = healEvent.Command;
            _eventPool.Return(healEvent);

            return (cancel, command);
        }
    }
}