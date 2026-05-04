namespace Game.Scripts.Battle.DamageDealing.Health.Data
{
    public struct HealCommand
    {
        public readonly IBattleUnit HealOrigin;
        public int Heal;

        public HealCommand(IBattleUnit healOrigin, int heal)
        {
            HealOrigin = healOrigin;
            Heal = heal;
        }
    }
}