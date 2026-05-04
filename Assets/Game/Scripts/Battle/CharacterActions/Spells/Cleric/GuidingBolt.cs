using Game.Scripts.Battle.StatusEffects.Implementations;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Spells.Cleric
{
    public class GuidingBolt : MonoStatusEffect
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
            _bus.Subscribe<AttackRollingEvent>(OnAttackingRoll)
                .When(x => x.Attacker == _unit);
        }

        protected override void OnDeactivate()
        {
            _bus.Unsubscribe<AttackRollingEvent>(OnAttackingRoll);
        }

        private void OnAttackingRoll(AttackRollingEvent ev)
        {
            ev.Parameters.AddAdvantage();
            if (_unit.GameObject.TryGetComponent(out IStatusEffectProcessor statusEffectProcessor))
                statusEffectProcessor.Detach(this);
        }
    }
}