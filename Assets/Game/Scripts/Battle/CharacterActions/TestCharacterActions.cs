using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.DataStorage.FreqDataStorage;
using Game.Scripts.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions
{
    public class TestCharacterActions : MonoBehaviour
    {
        [SerializeField] private AssetReferenceObject<MonoAction>[] _actions;
        [Inject] private IAsyncOperationScope _scope;
        [Inject] private IDataStorage _storage;

        private ICharacterActionsHolder _holder;

        private void Start()
        {
            if (!gameObject.TryGetComponent(out _holder))
                return;
            
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(StartAsync(ct));
        }

        private async UniTask StartAsync(CancellationToken ct)
        {
            foreach (var reference in _actions)
            {
                var prefab = await _storage.LoadObject(reference);
                _holder.Add(prefab);
            }
        }
    }
}