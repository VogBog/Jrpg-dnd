using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Pool.Interfaces;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Pool.Implementations
{
    public class ObjectPool : IObjectPool
    {
        [Inject] private DiContainer _container;
        
        private readonly Dictionary<Type, (Object, AsyncOperationHandle, int)> _prefabs = new();
        private readonly Dictionary<Type, Queue<object>> _pool = new();
        
        public async UniTask RegisterPrefab<T>(AssetReferenceObject<T> prefab, CancellationToken ct)
        {
            if (_prefabs.TryGetValue(typeof(T), out var parameters))
            {
                parameters.Item3++;
                return;
            }
            
            var handle = Addressables.LoadAssetAsync<Object>(prefab);
            var asset = await handle.ToUniTask(cancellationToken: ct);

            if (ct.IsCancellationRequested)
            {
                if (handle.IsValid()) handle.Release();
                throw new OperationCanceledException();
            }
            
            _prefabs.Add(typeof(T), (asset, handle, 1));
            _pool.Add(typeof(T), new Queue<object>());
        }

        public async UniTask RegisterPrefab(Type type, AssetReferenceObject<Object> prefab, CancellationToken ct)
        {
            if (_prefabs.TryGetValue(type, out var parameters))
            {
                parameters.Item3++;
                return;
            }
            
            var handle = Addressables.LoadAssetAsync<Object>(prefab);
            var asset = await handle.ToUniTask(cancellationToken: ct);

            if (ct.IsCancellationRequested)
            {
                if (handle.IsValid()) handle.Release();
                throw new OperationCanceledException();
            }
            
            _prefabs.Add(type, (asset, handle, 1));
            _pool.Add(type, new Queue<object>());
        }

        public async UniTask<T> Get<T>(CancellationToken ct)
        {
            if (!_pool.TryGetValue(typeof(T), out var queue))
                throw new NullReferenceException(
                    $"ObjectPool.Get<{typeof(T)}>: Cannot get a component. Prefab doesn't registered");

            if (queue.Count > 0)
            {
                if (queue.Dequeue() is T t)
                    return t;

                throw new NullReferenceException($"ObjectPool.Get<{typeof(T)}>: Cannot find component on the object");
            }

            var (prefab, _, _) = _prefabs[typeof(T)];
            var instance = _container.InstantiatePrefab(prefab);
            if (!instance.TryGetComponent(out T component))
                throw new NullReferenceException($"ObjectPool.Get<{typeof(T)}>: Cannot find component on the object");

            return component;
        }

        public void Return<T>(T asset)
        {
            if (!_pool.TryGetValue(typeof(T), out var queue))
                throw new NullReferenceException(
                    $"ObjectPool.Return<{typeof(T)}>: Cannot return object. Prefab doesn't registered in the pool");
            
            queue.Enqueue(asset);
        }

        public void ReleasePrefab<T>()
        {
            ReleasePrefab(typeof(T));
        }

        public void ReleasePrefab(Type type)
        {
            if (!_prefabs.TryGetValue(type, out var parameters))
                throw new NullReferenceException(
                    $"ObjectPool.Return<{type}>: Cannot release prefab. Prefab doesn't registered in the pool");

            parameters.Item3--;
            if (parameters.Item3 == 0)
            {
                if (parameters.Item2.IsValid())
                    parameters.Item2.Release();
                _prefabs.Remove(type);
            }
        }

        public void Dispose()
        {
            while (_prefabs.Keys.Count > 0)
                ReleasePrefab(_prefabs.Keys.First());
        }
    }
}