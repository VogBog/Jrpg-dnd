using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Battle.InitiativeQueue
{
    public class InitiativeRolledEvent
    {
        public IBattleUnit Unit { get; private set; }
        public RollResult Value { get; private set; }

        public InitiativeRolledEvent Init(IBattleUnit unit, RollResult value)
        {
            Unit = unit;
            Value = value;

            return this;
        }
    }
}