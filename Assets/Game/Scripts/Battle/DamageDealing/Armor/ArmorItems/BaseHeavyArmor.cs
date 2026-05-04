namespace Game.Scripts.Battle.DamageDealing.Armor.ArmorItems
{
    public class BaseHeavyArmor : IArmorItem
    {
        public IBattleUnit OwnerUnit { get; }
        public ArmorItemType ArmorType => ArmorItemType.Heavy;

        public BaseHeavyArmor(IBattleUnit owner)
        {
            OwnerUnit = owner;
        }
        
        public int GetArmorValue()
        {
            return 16;
        }
    }
}