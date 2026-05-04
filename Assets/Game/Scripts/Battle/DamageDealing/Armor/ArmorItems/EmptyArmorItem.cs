using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Characters.Stats.Interfaces;
using Zenject;

namespace Game.Scripts.Battle.DamageDealing.Armor.ArmorItems
{
    public class EmptyArmorItem : IArmorItem
    {
        private IStatsTable _statsTable;
        
        public IBattleUnit OwnerUnit { get; private set; }
        public ArmorItemType ArmorType => ArmorItemType.None;

        [Inject]
        private void Construct(IBattleUnit unit)
        {
            OwnerUnit = unit;
            _statsTable = unit.GameObject.GetComponent<IStatsTable>();
        }
        
        public int GetArmorValue()
        {
            if (_statsTable != null)
                return 10 + _statsTable.GetModifier(Stats.DEX);
            return 10;
        }
    }
}