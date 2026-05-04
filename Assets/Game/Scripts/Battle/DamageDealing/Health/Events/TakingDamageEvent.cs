using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;

namespace Game.Scripts.Battle.DamageDealing.Health.Events
{
    public class TakingDamageEvent
    {
        public IHealthProcessor Health { get; private set; }
        public IBattleUnit Unit { get; private set; }
        public DealDamageCommand Command;
        
        public bool IsCancelled { get; set; }

        public TakingDamageEvent SetData(IHealthProcessor health, IBattleUnit unit, DealDamageCommand command)
        {
            Health = health;
            Unit = unit;
            Command = command;
            IsCancelled = false;

            return this;
        }

        public void Clear()
        {
            Health = null;
            Unit = null;
            Command = default;
            IsCancelled = false;
        }
    }
}