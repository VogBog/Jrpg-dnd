namespace Game.Scripts.Battle.DamageDealing.Health.Events
{
    public class DiedEvent
    {
        public IBattleUnit Unit { get; private set; }

        public DiedEvent Init(IBattleUnit unit)
        {
            Unit = unit;

            return this;
        }
    }
}