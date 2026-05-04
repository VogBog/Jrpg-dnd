using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Events
{
    public class AttackRolledEvent
    {
        public AttackRollingEvent RollingEvent { get; private set; }
        public int Result;
        public bool CriticalHit;
        public bool CriticalMiss;

        public AttackRolledEvent Init(AttackRollingEvent ev, RollResult result)
        {
            RollingEvent = ev;
            Result = result.Result;
            CriticalMiss = result.CriticalOne;
            CriticalHit = result.CriticalHigh;

            return this;
        }

        public AttackRolledEvent AddBonus(int bonus)
        {
            Result += bonus;
            return this;
        }

        public AttackRolledEvent SetCriticalHit(bool criticalHit)
        {
            CriticalHit = criticalHit;
            return this;
        }

        public AttackRolledEvent SetCriticalMiss(bool criticalMiss)
        {
            CriticalMiss = criticalMiss;
            return this;
        }
    }
}