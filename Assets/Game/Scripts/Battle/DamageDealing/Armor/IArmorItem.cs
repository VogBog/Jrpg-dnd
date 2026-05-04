namespace Game.Scripts.Battle.DamageDealing.Armor
{
    public interface IArmorItem
    {
        IBattleUnit OwnerUnit { get; }
        ArmorItemType ArmorType { get; }
        
        int GetArmorValue();
    }
}