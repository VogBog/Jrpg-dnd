using UnityEngine;

namespace Game.Scripts.Battle.DamageDealing.Weapon
{
    [CreateAssetMenu(menuName = "Data/DamageDealing/WeaponType")]
    public class WeaponType : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
    }
}