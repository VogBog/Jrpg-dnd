using Cysharp.Threading.Tasks;
using Game.Scripts.DataStorage.FreqDataStorage;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Battle.DamageDealing.Damage
{
    [CreateAssetMenu(menuName = "Data/DamageDealing/DamageType")]
    public class DamageType : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AssetReferenceSprite IconReference { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }

        public Sprite Icon { get; private set; }

        public async UniTask<Sprite> Map(IDataStorage storage)
        {
            Icon = await storage.LoadT(IconReference);
            return Icon;
        }
    }
}