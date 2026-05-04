using Game.Scripts.Battle.DamageDealing.Health.Data;

namespace Game.Scripts.Battle.DamageDealing.Health.Events
{
    public class DyingEvent
    {
        public IBattleUnit Unit { get; private set; }
        public DealDamageCommand Command { get; private set; }

        public DyingEvent Init(IBattleUnit unit, DealDamageCommand command)
        {
            Unit = unit;
            Command = command;
            
            return this;
        }
    }
}