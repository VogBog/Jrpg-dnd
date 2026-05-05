using Game.Scripts.Battle;

namespace Game.Scripts.Rolls.Data
{
    public struct AttackRollResults
    {
        public int Roll;
        public bool Success;
        public bool CriticalHit;
        public bool CriticalMiss;
        public IBattleUnit Target;

        public AttackRollResults(IBattleUnit target, int roll, bool success, bool criticalHit, bool criticalMiss)
        {
            Target = target;
            Roll = roll;
            Success = success;
            CriticalHit = criticalHit;
            CriticalMiss = criticalMiss;
        }
    }
}