using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Characters.ModularCharacters;
using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.DataStorage.FreqDataStorage;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Battle.DamageDealing.Weapon
{
    [CreateAssetMenu(menuName = "Data/DamageDealing/WeaponData")]
    public class WeaponItem : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        
        [field: SerializeField] public AssetReferenceT<GameObject> Model { get; private set; }
        [field: SerializeField] public AssetReferenceT<AttackAnimationData> AttackAnimation { get; private set; }
        [field: SerializeField] public AssetReferenceT<IdleAnimationData> IdleAnimation { get; private set; }
        [field: SerializeField] public MapAssetReferenceT<WeaponType>[] Tags { get; private set; }
        [field: SerializeField] public Stats[] UsingStats { get; private set; }
        [field: SerializeField] public DamageListSerializable DamageList { get; private set; }
        [field: SerializeField] public int DamageBonus { get; private set; }
        [field: SerializeField] public int AttackBonus { get; private set; }

        public async UniTask Map(IDataStorage storage, CancellationToken ct)
        {
            foreach (var tag in Tags)
            {
                await tag.Map(storage, ct);
                ct.ThrowIfCancellationRequested();
            }
                
            await DamageList.Map(storage, ct);
        }
    }
}