namespace Game.Scripts.Rolls.Data
{
    public struct AttackRollResults
    {
        public int Roll;
        public bool Success;
        public bool CriticalHit;
        public bool CriticalMiss;

        public AttackRollResults(int roll, bool success, bool criticalHit, bool criticalMiss)
        {
            Roll = roll;
            Success = success;
            CriticalHit = criticalHit;
            CriticalMiss = criticalMiss;
        }
    }
}