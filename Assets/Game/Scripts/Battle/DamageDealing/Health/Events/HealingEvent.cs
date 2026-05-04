using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;

namespace Game.Scripts.Battle.DamageDealing.Health.Events
{
    public class HealingEvent
    {
        public IBattleUnit Unit { get; private set; }
        public IHealthProcessor Health { get; private set; }
        public HealCommand Command;
        public bool IsCancelled { get; set; }

        public HealingEvent SetData(IHealthProcessor health, IBattleUnit unit, HealCommand command)
        {
            Health = health;
            Unit = unit;
            Command = command;
            IsCancelled = false;

            return this;
        }

        public void Clear()
        {
            Unit = null;
            Health = null;
            Command = default;
            IsCancelled = false;
        }
    }
}