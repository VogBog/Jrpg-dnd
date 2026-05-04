using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.StatusEffects.Events;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Scenes;
using Game.Scripts.UI.Helpers;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Effects.VisualEffects
{
    public class StatusEffectIndicateCreator : MonoBehaviour
    {
        [Inject] private IAsyncOperationScope _scope;
        [Inject] private IEventBus _bus;
        
        [SerializeField] private UIObjectsPool<StatusEffectIndicatorView> _pool;
        [SerializeField] private float _showDuration;

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.InitializeAsync(ct));
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        private void OnEnable()
        {
            _bus.Subscribe<AttachedStatusEffectEvent>(OnStatusEffectAttached);
            _bus.Subscribe<DetachedStatusEffectEvent>(OnStatusEffectDetached);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<AttachedStatusEffectEvent>(OnStatusEffectAttached);
            _bus.Unsubscribe<DetachedStatusEffectEvent>(OnStatusEffectDetached);
        }

        private void OnStatusEffectAttached(AttachedStatusEffectEvent ev)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(ShowInfoAsync(
                    ev.Effect,
                    "Activates effect",
                    ct));
        }

        private void OnStatusEffectDetached(DetachedStatusEffectEvent ev)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(ShowInfoAsync(
                    ev.Effect,
                    "Effect ended",
                    ct));
        }

        private async UniTask ShowInfoAsync(IStatusEffect effect, string text, CancellationToken ct)
        {
            var item = await _pool.GetAsync(ct);
            item.Name.text = effect.Name;
            item.Underline.text = text;

            if (effect.Icon == null)
            {
                for (int i = 0; i < 4 && effect.Icon == null; i++)
                {
                    await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
                }
            }
            
            item.Icon.sprite = effect.Icon;
            item.Icon.color = effect.Icon == null ? new Color(1, 1, 1, 0) : Color.white;
            item.gameObject.SetActive(true);

            await UniTask.WaitForSeconds(_showDuration, cancellationToken: ct);
            
            _pool.Return(item);
        }
    }
}