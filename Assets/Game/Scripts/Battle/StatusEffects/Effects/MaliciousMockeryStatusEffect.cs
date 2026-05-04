using Game.Scripts.Battle.StatusEffects.Implementations;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using Zenject;

namespace Game.Scripts.Battle.StatusEffects.Effects
{
    public class MaliciousMockeryStatusEffect : MonoStatusEffect
    {
        [Inject] private IEventBus _bus;
        
        private IBattleUnit _target;
        
        public override void OnAdded(IBattleUnit owner)
        {
            _target = owner;
        }

        public override void OnRemoved()
        {
            _target = null;
        }

        protected override void OnActivate()
        {
            _bus.Subscribe<AttackRollingEvent>(OnAttackRoll)
                .When(e => e.Attacker == _target);
        }

        protected override void OnDeactivate()
        {
            _bus.Unsubscribe<AttackRollingEvent>(OnAttackRoll);
        }

        private void OnAttackRoll(AttackRollingEvent ev)
        {
            ev.Parameters.AddDisadvantage();
            Processor.Detach(this);
        }
    }
}