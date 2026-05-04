using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Battle.DamageDealing.Damage
{
    public struct DamageValue
    {
        public DiceParameters Dices;
        public DamageType DamageType;
        public int DamageBonus;

        public DamageValue(DiceParameters dices, DamageType damageType, int damageBonus = 0)
        {
            Dices = dices;
            DamageType = damageType;
            DamageBonus = damageBonus;
        }
    }
}