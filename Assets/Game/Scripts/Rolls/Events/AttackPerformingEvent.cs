using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Events
{
    public class AttackPerformingEvent
    {
        public AttackRolledEvent RolledEvent { get; private set; }
        public AttackRollResults Results { get; private set; }

        public AttackPerformingEvent Init(AttackRolledEvent rolledEvent, AttackRollResults result)
        {
            RolledEvent = rolledEvent;
            Results = result;

            return this;
        }
    }
}