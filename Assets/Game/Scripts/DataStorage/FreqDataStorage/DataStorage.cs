using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Game.Scripts.DataStorage.FreqDataStorage
{
    public class DataStorage : MonoBehaviour, IDataStorage
    {
        private readonly Dictionary<string, (Object, AssetReference)> _references = new();
        private readonly List<string> _referencesLoadingNow = new();

        public bool Loading => _referencesLoadingNow.Count > 0;
        
        public async UniTask<T> LoadT<T>(AssetReferenceT<T> reference) where T : Object
        {
            var result = await Load(reference);
            return result as T;
        }

        public async UniTask<Object> Load(AssetReference reference)
        {
            if (_references.TryGetValue(reference.AssetGUID, out var result))
                return result.Item1;

            if (_referencesLoadingNow.Contains(reference.AssetGUID))
            {
                await UniTask.WaitWhile(() => _referencesLoadingNow.Contains(reference.AssetGUID));
                if (_references.TryGetValue(reference.AssetGUID, out result))
                    return result.Item1;
            }

            try 
            {
                _referencesLoadingNow.Add(reference.AssetGUID);
                var asset = await reference.LoadAssetAsync<Object>();
                _references.Add(reference.AssetGUID, (asset, reference));
                return asset;
            }
            finally
            {
                _referencesLoadingNow.Remove(reference.AssetGUID);
            }
        }

        public async UniTask<T> LoadObject<T>(AssetReferenceObject<T> reference) where T : class
        {
            var result = await Load(reference);
            if (result == null)
                return null;
            
            if (result is GameObject go)
                return go.GetComponent<T>();

            return result as T;
        }

        public void ReleaseByType<T>()
        {
            var toRemove = new List<string>();
            foreach (var kvp in _references)
            {
                var (obj, reference) = kvp.Value;
                if (obj is T && reference.IsValid())
                {
                    reference.ReleaseAsset();
                    toRemove.Add(reference.AssetGUID);
                }
            }
            
            foreach (var remove in toRemove)
                _references.Remove(remove);
        }

        public void ReleaseAll()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (var (_, reference) in _references.Values)
            {
                if (reference != null && reference.IsValid())
                    reference.ReleaseAsset();
            }
            
            _references.Clear();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}