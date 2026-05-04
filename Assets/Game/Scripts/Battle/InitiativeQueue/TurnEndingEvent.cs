namespace Game.Scripts.Battle.InitiativeQueue
{
    public class TurnEndingEvent
    {
        public InitiativeUnitData UnitData { get; private set; }

        public TurnEndingEvent Init(InitiativeUnitData unitData)
        {
            UnitData = unitData;

            return this;
        }
    }
}