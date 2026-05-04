using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Battle.StatusEffects.Implementations
{
    public abstract class MonoStatusEffect : MonoBehaviour, IStatusEffect
    {
        [SerializeField] private AssetReferenceSprite _iconReference;
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        
        public Sprite Icon { get; private set; }
        public bool IsActive { get; private set; }
        public IStatusEffectProcessor Processor { get; private set; }

        [Inject]
        private void Construct(IAsyncOperationScope scope)
        {
            if (scope.TryGetToken(out var ct))
                scope.FireAndForget(OnInitialize(ct));
        }

        public void SetProcessor(IStatusEffectProcessor processor)
        {
            Processor = processor;
        }

        public void Activate()
        {
            IsActive = true;
            OnActivate();
        }

        public void Deactivate()
        {
            IsActive = false;
            OnDeactivate();
        }

        public abstract void OnAdded(IBattleUnit owner);
        public abstract void OnRemoved();
        protected abstract void OnActivate();
        protected abstract void OnDeactivate();

        private async UniTask OnInitialize(CancellationToken ct)
        {
            Icon = await _iconReference.LoadAssetAsync();
        }

        private void OnDestroy()
        {
            if (_iconReference.IsValid())
                _iconReference.ReleaseAsset();
        }
    }
}