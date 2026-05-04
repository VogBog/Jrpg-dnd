using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.DataStorage.FreqDataStorage;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Game.Helpers.AddressablesHelpers
{
    [Serializable]
    public class MapAssetReferenceT<T> where T : Object
    {
        [SerializeField] private AssetReferenceT<T> _reference;

        [HideInInspector] public T Value;

        public async UniTask<T> Map(IDataStorage storage, CancellationToken ct)
        {
            Value = await storage.LoadT(_reference);
            return Value;
        }
    }
}