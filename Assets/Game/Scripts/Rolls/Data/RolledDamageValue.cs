using Game.Scripts.Battle.DamageDealing.Damage;

namespace Game.Scripts.Rolls.Data
{
    public struct RolledDamageValue
    {
        public DamageType DamageType;
        public int Value;

        public RolledDamageValue(DamageType damageType, int value)
        {
            DamageType = damageType;
            Value = value;
        }
    }
}