using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Battle.DamageDealing.Health.Data
{
    public struct DealDamageCommand
    {
        public readonly IBattleUnit DamageDealer;
        public RolledDamageList DamageList;
        public bool IsCritical;

        public DealDamageCommand(IBattleUnit damageDealer, RolledDamageList damageList, bool isCritical)
        {
            DamageDealer = damageDealer;
            DamageList = damageList;
            IsCritical = isCritical;
        }
    }
}