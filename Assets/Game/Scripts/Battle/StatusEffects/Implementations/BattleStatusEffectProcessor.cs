using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.StatusEffects.Data;
using Game.Scripts.Battle.StatusEffects.Events;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Events.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.StatusEffects.Implementations
{
    public class BattleStatusEffectProcessor : MonoBehaviour, IStatusEffectProcessor
    {
        private IEventBus _bus;
        private IEventPool _pool;

        private readonly List<TempEffectData> _effects = new();
        
        public IBattleUnit Owner { get; private set; }

        public event Action<IStatusEffectProcessor> Changed;

        [Inject]
        private void Construct(IBattleUnit unit, IEventBus bus, IEventPool pool)
        {
            Owner = unit;
            _bus = bus;
            _pool = pool;
        }
        
        public void Attach(AttachStatusEffectCommand command)
        {
            var obj = Owner.UnitContainer.InstantiatePrefab(command.Prefab);
            if (!obj.TryGetComponent(out IStatusEffect effect))
                throw new NullReferenceException(
                    $"BattleStatusEffectProcessor.Attach: prefab {command.Prefab} doesn't have IPassiveEffect component");

            bool isDuplicate = false;
            for (int i = 0; i < _effects.Count; i++)
            {
                var effectData = _effects[i];
                if (effectData.Command.Prefab == command.Prefab)
                {
                    effectData.HasDuplicate = true;
                    _effects[i] = effectData;
                    isDuplicate = true;
                    break;
                }
            }
            
            var data = new TempEffectData(command, obj, effect, command.EndEffectTime, command.Rounds, isDuplicate);
            if (command.EndEffectTime is EndEffectTimes.TurnEnd)
                ++data.Rounds;
            
            _effects.Add(data);
            
            effect.SetProcessor(this);
            effect.OnAdded(Owner);
            if (!isDuplicate)
                effect.Activate();

            var ev = _pool.Get<AttachedStatusEffectEvent>().Init(command, Owner, effect);
            _bus.Publish(ev);
            _pool.Return(ev);
        }

        public void Detach(AttachStatusEffectCommand command)
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                var effectData = _effects[i];
                if (effectData.Command.Prefab == command.Prefab &&
                    effectData.Command.Caster == command.Caster)
                {
                    RemoveAt(i);
                    break;
                }
            }
        }

        public void Detach(IStatusEffect effect)
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                var effectData = _effects[i];
                if (effectData.Effect == effect)
                {
                    RemoveAt(i);
                    break;
                }
            }
        }

        public IEnumerable<IStatusEffect> GetAll(bool onlyActive)
        {
            return _effects.Select(x => x.Effect).Where(x => x.IsActive || !onlyActive);
        }

        private void RemoveAt(int index)
        {
            var effectData = _effects[index];
            _effects.RemoveAt(index);
                    
            if (effectData.IsDuplicate)
                OnRemoveDuplicate(effectData);
            else if (effectData.HasDuplicate)
                OnRemoveMainDuplicated(effectData);
            
            if (effectData.Effect.IsActive)
                effectData.Effect.Deactivate();
            effectData.Effect.OnRemoved();

            var ev = _pool.Get<DetachedStatusEffectEvent>().Init(Owner, effectData.Effect);
            _bus.Publish(ev);
            _pool.Return(ev);
        }

        private void OnRemoveDuplicate(TempEffectData removed)
        {
            bool hasDuplicates = false;
            int mainIndex = -1;
            for (int i = 0; i < _effects.Count; i++)
            {
                var effectData = _effects[i];
                if (effectData.Command.Prefab == removed.Command.Prefab)
                {
                    if (effectData.HasDuplicate)
                        mainIndex = i;
                    else
                        hasDuplicates = true;
                }
            }
            
            var effect = _effects[mainIndex];
            effect.HasDuplicate = hasDuplicates;
            _effects[mainIndex] = effect;
        }

        private void OnRemoveMainDuplicated(TempEffectData removed)
        {
            int mainIndex = -1;
            bool hasDuplicates = false;
            for (int i = 0; i < _effects.Count; i++)
            {
                var effectData = _effects[i];
                if (effectData.Command.Prefab == removed.Command.Prefab)
                {
                    if (mainIndex == -1)
                        mainIndex = i;
                    else 
                        hasDuplicates = true;
                }
            }

            if (mainIndex == -1)
                return;
            
            var effect = _effects[mainIndex];
            effect.HasDuplicate = hasDuplicates;
            effect.IsDuplicate = false;
            _effects[mainIndex] = effect;
            
            if (!effect.Effect.IsActive)
                effect.Effect.Activate();
        }

        private void OnEnable()
        {
            _bus.Subscribe<TurnStartingEvent>(OnTurnStarting)
                .When(e => e.UnitData.Unit == Owner);
            _bus.Subscribe<TurnEndingEvent>(OnTurnEnding)
                .When(e => e.UnitData.Unit == Owner);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<TurnStartingEvent>(OnTurnStarting);
            _bus.Unsubscribe<TurnEndingEvent>(OnTurnEnding);
        }

        private void OnTurnStarting(TurnStartingEvent ev)
        {
            CheckForRemove(EndEffectTimes.TurnStart);
        }

        private void OnTurnEnding(TurnEndingEvent ev)
        {
            CheckForRemove(EndEffectTimes.TurnEnd);
        }

        private void CheckForRemove(EndEffectTimes time)
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                var effect = _effects[i];
                if (effect.EndEffectTime == time)
                {
                    effect.Rounds--;
                    if (effect.Rounds <= 0)
                    {
                        RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        _effects[i] = effect;
                    }
                }
            }
        }
    }
}