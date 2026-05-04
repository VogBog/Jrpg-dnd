namespace Game.Scripts.Battle.InitiativeQueue
{
    public struct InitiativeUnitData
    {
        public readonly IBattleUnit Unit;
        public int Initiative;

        public InitiativeUnitData(IBattleUnit unit, int value)
        {
            Unit = unit;
            Initiative = value;
        }
    }
}