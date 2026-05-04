using Game.Scripts.Battle.StatusEffects.Implementations;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.GoblinActions
{
    public class RevengeOfTheSmallsStatusEffect : MonoStatusEffect
    {
        [SerializeField] private int _damageBonus;
        
        private IBattleUnit _owner;
        private IEventBus _bus;

        [Inject]
        private void Construct(IEventBus bus)
        {
            _bus = bus;
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
            _bus.Subscribe<AttackRollingEvent>(OnAttackRolling)
                .When(x => x.Attacker == _owner);
            _bus.Subscribe<DamageRollingEvent>(OnDamageRolling)
                .When(x => x.Attacker == _owner);
        }

        protected override void OnDeactivate()
        {
            _bus.Unsubscribe<AttackRollingEvent>(OnAttackRolling);
            _bus.Unsubscribe<DamageRollingEvent>(OnDamageRolling);
        }

        private void OnAttackRolling(AttackRollingEvent ev)
        {
            ev.Parameters.AddAdvantage();
        }

        private void OnDamageRolling(DamageRollingEvent ev)
        {
            var value = ev.DamageList.Values[0];
            value.DamageBonus += 2;
            ev.DamageList.Values[0] = value;
        }
    }
}