using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.DataStorage.FreqDataStorage;
using Game.Scripts.Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Characters.CharacterResources.Implementations
{
    public class DefaultCharacterResourcesHolder : CharacterResourcesHolder
    {
        [SerializeField] private AssetReferenceT<CharacterResourcesList> _resourcesList;

        [Inject]
        private void Construct(IAsyncOperationScope scope, IDataStorage storage)
        {
            if (scope.TryGetToken(out var ct))
                scope.FireAndForget(LoadResources(storage, ct));
        }

        private async UniTask LoadResources(IDataStorage storage, CancellationToken ct)
        {
            var handle = _resourcesList.LoadAssetAsync();
            var list = await handle.ToUniTask(cancellationToken: ct);

            foreach (var resource in list.Resources)
            {
                var asset = await storage.LoadT(resource);
                Add(asset, 1);
                
                ct.ThrowIfCancellationRequested();
            }

            foreach (var resource in list.MarkedResources)
            {
                var asset = await storage.LoadT(resource.Resource);
                AddMarked(asset, 1, resource.MarkValue);
                
                ct.ThrowIfCancellationRequested();
            }
            
            if (handle.IsValid())
                handle.Release();
        }
    }
}