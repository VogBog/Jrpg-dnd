using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.UI.Helpers
{
    [Serializable]
    public class UIObjectsPool<T> : IDisposable where T : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private AssetReferenceObject<T> _prefabReference;
        
        private T _prefab;
        private bool _loading = true;

        private readonly List<T> _usedItems = new();
        private readonly Queue<T> _freeItems = new();
        
        public Transform Container => _container;
        public List<T> UsingItems => _usedItems;

        public event Action<T> InstantiatedOne; 

        public async UniTask InitializeAsync(CancellationToken ct, int preInstantiateCount = 0)
        {
            _prefab = await _prefabReference.LoadAssetAsync();
            if (ct.IsCancellationRequested)
            {
                _prefab = null;
                _prefabReference.ReleaseIfValid();
                return;
            }

            for (int i = 0; i < preInstantiateCount; i++)
            {
                _freeItems.Enqueue(Instantiate());
            }

            _loading = false;
        }

        public async UniTask<T> GetAsync(CancellationToken ct)
        {
            var item = await GetFromFreeItems(ct);
            _usedItems.Add(item);
            return item;
        }

        public void Return(T item)
        {
            _usedItems.Remove(item);
            _freeItems.Enqueue(item);
            item.gameObject.SetActive(false);
        }

        public void ReturnAll()
        {
            foreach (var item in _usedItems)
            {
                item.gameObject.SetActive(false);
                _freeItems.Enqueue(item);
            }
            
            _usedItems.Clear();
        }

        public void Dispose()
        {
            _prefabReference.ReleaseIfValid();
        }

        private T Instantiate()
        {
            var instance = Object.Instantiate(_prefab, _container);
            instance.gameObject.SetActive(false);
            InstantiatedOne?.Invoke(instance);

            return instance;
        }

        private async UniTask<T> GetFromFreeItems(CancellationToken ct)
        {
            if (_loading)
                await UniTask.WaitWhile(() => _loading, cancellationToken: ct);

            if (_freeItems.Count > 0)
                return _freeItems.Dequeue();

            return Instantiate();
        }
    }
}