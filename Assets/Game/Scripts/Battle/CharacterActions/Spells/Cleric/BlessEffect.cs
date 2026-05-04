using Game.Scripts.Battle.StatusEffects.Implementations;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Spells.Cleric
{
    public class BlessEffect : MonoStatusEffect
    {
        private IBattleUnit _unit;
        [Inject] private IEventBus _bus;
        
        public override void OnAdded(IBattleUnit owner)
        {
            _unit = owner;
        }

        public override void OnRemoved()
        {
            _unit = null;
        }

        protected override void OnActivate()
        {
            _bus.Subscribe<AttackRollingEvent>(OnAttackRolling)
                .When(x => x.Attacker == _unit);
            _bus.Subscribe<SaveThrowRollingEvent>(OnSaveThrowRolling)
                .When(x => x.Target == _unit);
        }

        protected override void OnDeactivate()
        {
            _bus.Unsubscribe<AttackRollingEvent>(OnAttackRolling);
            _bus.Unsubscribe<SaveThrowRollingEvent>(OnSaveThrowRolling);
        }

        private void OnAttackRolling(AttackRollingEvent ev)
        {
            ev.Parameters.AddDices(1, 4);
        }

        private void OnSaveThrowRolling(SaveThrowRollingEvent ev)
        {
            ev.Parameters.AddDices(1, 4);
        }
    }
}