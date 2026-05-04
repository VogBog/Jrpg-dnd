using Game.Scripts.Characters.Stats.Interfaces;

namespace Game.Scripts.Battle.DamageDealing.Armor.ArmorItems
{
    public class BaseLightArmor : IArmorItem
    {
        public IBattleUnit OwnerUnit { get; }
        public IStatsTable Stats { get; }
        public ArmorItemType ArmorType => ArmorItemType.Light;

        public BaseLightArmor(IBattleUnit owner)
        {
            OwnerUnit = owner;
            Stats = OwnerUnit.GameObject.GetComponent<IStatsTable>();
        }
        
        public int GetArmorValue()
        {
            if (Stats == null)
                return 12;
            return Stats.GetModifier(Characters.Stats.Data.Stats.DEX) + 12;
        }
    }
}