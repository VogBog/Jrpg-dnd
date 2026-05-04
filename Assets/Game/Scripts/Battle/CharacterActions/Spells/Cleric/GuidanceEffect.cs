using Game.Scripts.Battle.StatusEffects.Implementations;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Spells.Cleric
{
    public class GuidanceEffect : MonoStatusEffect
    {
        private IBattleUnit _unit;
        private IEventBus _bus;

        [Inject]
        private void Construct(IEventBus bus)
        {
            _bus = bus;
        }
        
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
        }

        protected override void OnDeactivate()
        {
            _bus.Unsubscribe<AttackRollingEvent>(OnAttackRolling);
        }

        private void OnAttackRolling(AttackRollingEvent ev)
        {
            ev.Parameters.AddDices(1, 4);
        }
    }
}