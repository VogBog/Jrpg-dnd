using System;
using Game.Scripts.Battle.DamageDealing.Armor.ArmorItems;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.DamageDealing.Armor
{
    public class Armor : MonoBehaviour
    {
        [SerializeField] private ArmorItemType _giveMeArmor; //For test, TODO - change it
        
        private IBattleUnit _owner;
        
        public readonly SimpleAttribute Bonus = new();

        public IArmorItem WearingItem { get; private set; }
        public int Value => WearingItem.GetArmorValue() + Bonus.Value;

        public event Action<Armor> Changed;

        [Inject]
        private void Construct(IBattleUnit unit)
        {
            if (unit == null) return;
            
            _owner = unit;
            Bonus.Changed += _ => Changed?.Invoke(this);
            SetArmorItem<EmptyArmorItem>();
        }

        public void SetArmorItem<TArmor>() where TArmor : IArmorItem
        {
            SetArmorItem(typeof(TArmor));
        }

        public void SetArmorItem(Type type)
        {
            if (!typeof(IArmorItem).IsAssignableFrom(type))
                throw new ArgumentException($"Armor.SetArmorItem: Type {type} does not implement IArmorItem");

            if (_owner == null)
                throw new NullReferenceException("Armor.SetArmorItem: Owner of Armor is null.");

            if (_owner.UnitContainer.Instantiate(type) is not IArmorItem armorItem)
                throw new NullReferenceException($"Armor.SetArmorItem: Cannot create armor of type {type}");

            WearingItem = armorItem;
            Changed?.Invoke(this);
        }

        private void Awake()
        {
            if (_giveMeArmor is ArmorItemType.Light) SetArmorItem<BaseLightArmor>();
            else if(_giveMeArmor is ArmorItemType.Medium) SetArmorItem<BaseMediumArmor>();
            else if(_giveMeArmor is ArmorItemType.Heavy) SetArmorItem<BaseHeavyArmor>();
        }
    }
}