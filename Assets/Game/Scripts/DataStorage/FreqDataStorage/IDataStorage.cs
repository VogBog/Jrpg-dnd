using System;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Game.Scripts.DataStorage.FreqDataStorage
{
    public interface IDataStorage : IDisposable
    {
        bool Loading { get; }
        
        UniTask<T> LoadT<T>(AssetReferenceT<T> reference) where T : Object;
        UniTask<Object> Load(AssetReference reference);
        UniTask<T> LoadObject<T>(AssetReferenceObject<T> reference) where T : class;
        
        void ReleaseByType<T>();
        void ReleaseAll();
    }
}