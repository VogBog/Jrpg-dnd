namespace Game.Scripts.Battle.InitiativeQueue
{
    public class TurnStartingEvent
    {
        public InitiativeUnitData UnitData { get; private set; }

        public TurnStartingEvent Init(InitiativeUnitData unitData)
        {
            UnitData = unitData;

            return this;
        }
    }
}