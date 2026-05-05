using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle.DamageDealing.Health.Events;
using Game.Scripts.Events.Data;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Pool.Interfaces;
using Game.Scripts.Rolls.Data;
using Game.Scripts.Rolls.Events;
using Game.Scripts.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Effects.VisualEffects
{
    public class AttackEffectsCreator : MonoBehaviour
    {
        [Inject] private IObjectPool _pool;
        [Inject] private IEventBus _bus;
        [Inject] private IAsyncOperationScope _scope;
        
        [SerializeField] private AssetReferenceObject<TextPopupEffect> _textPopupEffectReference;
        [SerializeField] private Color _takeDamageColor;
        [SerializeField] private Color _missColor;
        [SerializeField] private Color _criticalMissColor;
        [SerializeField] private Color _criticalHitColor;
        [SerializeField] private Color _healColor;

        public const float YOffset = 0.5f;
        public const float SmallYOffset = 0.25f;

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.RegisterPrefab(_textPopupEffectReference, ct));
        }

        private void OnDestroy()
        {
            _pool.ReleasePrefab<TextPopupEffect>();
        }

        private void OnEnable()
        {
            _bus.Subscribe<AttackPerformingEvent>(OnAttackPerforming).On(EventStep.SeeResults);
            _bus.Subscribe<TakenDamageEvent>(OnTakeDamage).On(EventStep.SeeResults);
            _bus.Subscribe<HealingEvent>(OnHeal).On(EventStep.SeeResults);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<AttackPerformingEvent>(OnAttackPerforming);
            _bus.Unsubscribe<TakenDamageEvent>(OnTakeDamage);
            _bus.Unsubscribe<HealingEvent>(OnHeal);
        }

        private void OnAttackPerforming(AttackPerformingEvent ev)
        {
            var pos = ev.RolledEvent.RollingEvent.Target.GameObject.transform.position;
            if (!ev.Results.Success)
            {
                if (ev.Results.CriticalMiss)
                    PopupText(pos, "Critical Miss", _criticalMissColor);
                else 
                    PopupText(pos, "Miss", _missColor);
            }
        }

        private void OnTakeDamage(TakenDamageEvent ev)
        {
            var pos = ev.Unit.GameObject.transform.position;
            
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForgetDelayed(
                    PopupDamageAsync(pos, ev.Command.IsCritical, ev.Command.DamageList, ct));

            if (ev.Command.IsCritical)
            {
                pos.y += SmallYOffset;
                PopupText(pos, "CRITICAL HIT", _criticalHitColor);
            }
        }

        private void OnHeal(HealingEvent ev)
        {
            var pos = ev.Unit.GameObject.transform.position;
            PopupText(pos, $"+{ev.Command.Heal}", _healColor);
        }

        private void PopupText(Vector3 position, string text, Color color)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForgetDelayed(PopupTextAsync(position, text, color, ct));
        }

        private async UniTask PopupTextAsync(Vector3 position, string text, Color color, CancellationToken ct)
        {
            var effect = await _pool.Get<TextPopupEffect>(ct);
            effect.transform.position = position + YOffset * Vector3.up;
            await effect.PopupAsync(text, color, ct);
        }

        private async UniTask PopupDamageAsync(Vector3 position, bool isCritical, RolledDamageList damage, CancellationToken ct)
        {
            foreach (var value in damage.Values)
            {
                int totalDamage = value.Value;
                if (isCritical)
                    totalDamage *= 2;
                
                PopupTextForceAsync(position, totalDamage.ToString(), value.DamageType.Color, ct).Forget();
            }
        }

        private async UniTask PopupTextForceAsync(Vector3 position, string text, Color color, CancellationToken ct)
        {
            var effect = await _pool.Get<TextPopupEffect>(ct);
            effect.transform.position = position + YOffset * Vector3.up;
            await effect.PopupForced(text, color, ct);
            _pool.Return(effect);
        }
    }
}