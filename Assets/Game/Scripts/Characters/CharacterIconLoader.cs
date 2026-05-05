using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle;
using Game.Scripts.Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Characters
{
    public class CharacterIconLoader : MonoBehaviour
    {
        [Inject] private IBattleUnit _unit;
        [Inject] private IAsyncOperationScope _scope;
        
        [SerializeField] private AssetReferenceSprite _iconReference;
        
        public bool Loading { get; private set; }

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(LoadIconAsync(ct));
        }

        public async UniTask<Sprite> LoadIconAsync(CancellationToken ct)
        {
            Loading = true;
            var icon = await _iconReference.LoadAssetAsync().ToUniTask(cancellationToken: ct);
            _unit.Icon = icon;
            Loading = false;
            return icon;
        }

        private void OnDestroy()
        {
            if (_iconReference.IsValid())
                _iconReference.ReleaseAsset();
        }
    }
}