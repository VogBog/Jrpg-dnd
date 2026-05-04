using Game.Scripts.Characters.Stats.Interfaces;
using UnityEngine;

namespace Game.Scripts.Battle.DamageDealing.Armor.ArmorItems
{
    public class BaseMediumArmor : IArmorItem
    {
        public IBattleUnit OwnerUnit { get; }
        public IStatsTable Stats { get; }
        public ArmorItemType ArmorType => ArmorItemType.Medium;

        public BaseMediumArmor(IBattleUnit unit)
        {
            OwnerUnit = unit;
            Stats = OwnerUnit.GameObject.GetComponent<IStatsTable>();
        }
        
        public int GetArmorValue()
        {
            if (Stats == null)
                return 14;
            return 14 + Mathf.Max(Stats.GetModifier(Characters.Stats.Data.Stats.DEX), 2);
        }
    }
}