using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Events.Data;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Pool.Interfaces;
using Game.Scripts.Rolls.Events;
using Game.Scripts.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Effects.VisualEffects
{
    public class SaveThrowsEffectsCreator : MonoBehaviour
    {
        [SerializeField] private AssetReferenceObject<SaveThrowEffect> _saveThrowEffectReference;

        [SerializeField] private Color _successColor;
        [SerializeField] private Color _failureColor;
        [SerializeField] private Color _rollingColor;
        [SerializeField] private float _yOffset;

        private IObjectPool _pool;
        private IAsyncOperationScope _scope;
        private IEventBus _bus;
        
        [Inject]
        private void Construct(IObjectPool pool, IAsyncOperationScope scope, IEventBus bus)
        {
            _pool = pool;
            _scope = scope;
            _bus = bus;
        }

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.RegisterPrefab(_saveThrowEffectReference, ct));
        }

        private void OnDestroy()
        {
            _pool.ReleasePrefab<SaveThrowEffect>();   
        }

        private void OnEnable()
        {
            _bus.Subscribe<SaveThrowRolledEvent>(OnSaveThrowRolled).On(EventStep.SeeResults);
        }

        private void OnDisable()
        {
            _bus.Subscribe<SaveThrowRolledEvent>(OnSaveThrowRolled);
        }

        private void OnSaveThrowRolled(SaveThrowRolledEvent ev)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForgetDelayed(PlaySaveThrowEffect(ev, ct));
        }

        private async UniTask PlaySaveThrowEffect(SaveThrowRolledEvent ev, CancellationToken ct)
        {
            var effect = await _pool.Get<SaveThrowEffect>(ct);
            effect.transform.position = ev.RollingEvent.Target.GameObject.transform.position + Vector3.up * _yOffset;
            await effect.Animate(
                ev.Success ? "Success" : "Failure",
                ev.RollResult,
                _rollingColor,
                ev.Success ? _successColor : _failureColor,
                ev.Success ? _successColor : _failureColor,
                ct);
            _pool.Return(effect);
        }
    }
}