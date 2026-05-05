namespace Game.Scripts.Battle.InitiativeQueue
{
    public class TurnStartingEvent
    {
        public InitiativeUnitData UnitData { get; private set; }
        public bool IsFirstTurn { get; private set; }

        public TurnStartingEvent Init(InitiativeUnitData unitData, bool isFirstTurn)
        {
            UnitData = unitData;
            IsFirstTurn = isFirstTurn;

            return this;
        }
    }
}