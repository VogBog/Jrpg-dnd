#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Game.Helpers.AddressablesHelpers
{
    [Serializable]
    public class AssetReferenceObject<T> : AssetReference
    {
        private AsyncOperationHandle<Object> _handle;
        
        public T Result { get; private set; }
        
        public AssetReferenceObject(string guid) : base(guid)
        {
        }
        
        public Type AssetType => typeof(T);

        public override bool ValidateAsset(Object obj)
        {
#if UNITY_EDITOR
            if (obj == null)
                return false;

            if (obj is GameObject go)
                return go.TryGetComponent<T>(out _);
            if (obj is ScriptableObject)
                return obj is T;

            return false;
#else
            return false;
#endif
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var extension = System.IO.Path.GetExtension(path);
            if (extension != ".prefab" && extension != ".asset") 
                return false;
    
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            return ValidateAsset(obj);
#else
            return false;
#endif
        }

        public async UniTask<T> LoadAssetAsync()
        {
            var handle = Addressables.LoadAssetAsync<Object>(RuntimeKey);
            if (!handle.IsValid())
                throw new InvalidOperationException($"Failed to load asset AssetReferenceObject<{typeof(T).Name}>");
            
            await handle.ToUniTask();
            if(handle.Result == null)
                throw new InvalidOperationException($"Failed to load asset AssetReferenceObject<{typeof(T).Name}>");

            if (handle.Result is T tRes)
            {
                _handle = handle;
                Result = tRes;
                return tRes;
            }
            
            if(handle.Result is not GameObject go)
                throw new InvalidCastException(
                    $"Failed to load asset AssetReferenceObject<{typeof(T).Name}>. Asset is not GameObject");

            if (!go.TryGetComponent<T>(out var component))
                throw new InvalidOperationException(
                    $"Failed to load asset AssetReferenceObject<{typeof(T).Name}>. Cannot find component on GameObject");

            _handle = handle;
            Result = component;
            return component;
        }

        public void ReleaseIfValid()
        {
            if (_handle.IsValid())
                _handle.Release();
        }

        public AssetReferenceObject<T> GetCopy() => new(AssetGUID);
    }
}