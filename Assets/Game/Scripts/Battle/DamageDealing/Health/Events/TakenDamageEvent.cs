using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;

namespace Game.Scripts.Battle.DamageDealing.Health.Events
{
    public class TakenDamageEvent
    {
        public IHealthProcessor Health { get; private set; }
        public IBattleUnit Unit { get; private set; }
        public DealDamageCommand Command { get; private set; }
        public int TotalDamage { get; private set; }

        public TakenDamageEvent SetData(
            IHealthProcessor health,
            IBattleUnit unit,
            DealDamageCommand command,
            int totalDamage)
        {
            Health = health;
            Unit = unit;
            TotalDamage = totalDamage;
            Command = command;

            return this;
        }

        public void Clear()
        {
            Health = null;
            Unit = null;
            TotalDamage = 0;
            Command = default;
        }
    }
}