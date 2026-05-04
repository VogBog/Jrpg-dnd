using Game.Scripts.Battle.StatusEffects.Implementations;
using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using Zenject;

namespace Game.Scripts.Battle.StatusEffects.Effects
{
    public class DodgingStatusEffect : MonoStatusEffect
    {
        private IBattleUnit _owner;
        private IEventBus _eventBus;

        [Inject]
        private void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        public override void OnAdded(IBattleUnit owner)
        {
            _owner = owner;
        }

        public override void OnRemoved()
        {
            _owner = null;
        }

        protected override void OnActivate()
        {
            _eventBus.Subscribe<AttackRollingEvent>(OnAttackRolling)
                .When(x => x.Target == _owner);
            _eventBus.Subscribe<SaveThrowRollingEvent>(OnSavingThrows)
                .When(x => x.Target == _owner);
        }

        protected override void OnDeactivate()
        {
            _eventBus.Unsubscribe<AttackRollingEvent>(OnAttackRolling);
            _eventBus.Unsubscribe<SaveThrowRollingEvent>(OnSavingThrows);
        }

        private void OnAttackRolling(AttackRollingEvent ev)
        {
            ev.Parameters.AddDisadvantage();
        }

        private void OnSavingThrows(SaveThrowRollingEvent ev)
        {
            if (ev.ThrowingStat is Stats.DEX)
                ev.Parameters.AddAdvantage();
        }
    }
}