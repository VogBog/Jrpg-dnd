namespace Game.Scripts.Rolls.Events
{
    public class SaveThrowRolledEvent
    {
        public SaveThrowRollingEvent RollingEvent { get; private set; }
        public int RollResult { get; private set; }
        public bool Success;

        public SaveThrowRolledEvent Init(SaveThrowRollingEvent rollingEvent, int rollResult, bool success)
        {
            RollingEvent = rollingEvent;
            RollResult = rollResult;
            Success = success;

            return this;
        }
    }
}