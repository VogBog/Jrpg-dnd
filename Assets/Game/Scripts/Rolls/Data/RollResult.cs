namespace Game.Scripts.Rolls.Data
{
    public struct RollResult
    {
        public int Result;
        public bool CriticalOne;
        public bool CriticalHigh;

        public RollResult(int result, bool criticalOne, bool criticalHigh)
        {
            Result = result;
            CriticalOne = criticalOne;
            CriticalHigh = criticalHigh;
        }
    }
}