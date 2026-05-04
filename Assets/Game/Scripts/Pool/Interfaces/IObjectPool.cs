using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Object = UnityEngine.Object;

namespace Game.Scripts.Pool.Interfaces
{
    public interface IObjectPool : IDisposable
    {
        UniTask RegisterPrefab<T>(AssetReferenceObject<T> prefab, CancellationToken ct);
        UniTask RegisterPrefab(Type type, AssetReferenceObject<Object> prefab, CancellationToken ct);
        UniTask<T> Get<T>(CancellationToken ct);
        void Return<T>(T asset);
        void ReleasePrefab<T>();
        void ReleasePrefab(Type type);
    }
}