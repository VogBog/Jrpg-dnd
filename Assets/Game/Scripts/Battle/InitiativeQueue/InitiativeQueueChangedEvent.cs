using System.Collections.Generic;

namespace Game.Scripts.Battle.InitiativeQueue
{
    public readonly struct InitiativeQueueChangedEvent
    {
        public readonly IReadOnlyList<InitiativeUnitData> AllUnits;
        public readonly InitiativeUnitData ChangedUnit;
        public readonly int UnitIndex;

        public InitiativeQueueChangedEvent(
            IReadOnlyList<InitiativeUnitData> allUnits,
            InitiativeUnitData changedUnit,
            int unitIndex)
        {
            AllUnits = allUnits;
            ChangedUnit = changedUnit;
            UnitIndex = unitIndex;
        }
    }
}