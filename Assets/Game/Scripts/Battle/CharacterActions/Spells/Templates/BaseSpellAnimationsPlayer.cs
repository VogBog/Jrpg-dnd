using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle.BattleAnimations.Implementations;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Effects.VisualEffects;
using Game.Scripts.Pool.Interfaces;
using Game.Scripts.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Spells.Templates
{
    //Just for quick start
    public class BaseSpellAnimationsPlayer : MonoBehaviour
    {
        [SerializeField] private AssetReferenceObject<BaseSpellProjectile> _projectileReference;
        [SerializeField] private AssetReferenceObject<BaseSaveThrowEffect> _saveThrowEffectReference;
        [SerializeField] private float _projectileHitTime;
        [SerializeField] private float _projectileOutTime;

        [Inject] private IObjectPool _pool;
        [Inject] private IAsyncOperationScope _scope;

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(AwakeAsync(ct));
        }

        private async UniTask AwakeAsync(CancellationToken ct)
        {
            await _pool.RegisterPrefab(_projectileReference, ct);
            await _pool.RegisterPrefab(_saveThrowEffectReference, ct);
        }

        private void OnDestroy()
        {
            _pool.ReleasePrefab<BaseSpellProjectile>();
            _pool.ReleasePrefab<BaseSaveThrowEffect>();
        }

        public async UniTask AnimateProjectileAsync(
            Vector3 from,
            Vector3 to,
            DamageType damageType,
            CancellationToken ct,
            Func<UniTask> onDealDamage)
        {
            var projectile = await _pool.Get<BaseSpellProjectile>(ct);
            var main = projectile.ParticleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient(damageType.Color);

            projectile.transform.position = from;
            projectile.Throw(to);
            projectile.gameObject.SetActive(true);
            projectile.ParticleSystem.Play();

            await UniTask.WaitForSeconds(_projectileHitTime, cancellationToken: ct);

            await onDealDamage.Invoke();

            projectile.ParticleSystem.Stop();
            await UniTask.WaitForSeconds(_projectileOutTime, cancellationToken: ct);
            
            projectile.gameObject.SetActive(false);
            _pool.Return(projectile);
        }

        public async UniTask AnimateSaveThrow(
            Vector3 center,
            DamageType damageType,
            CancellationToken ct,
            Func<UniTask> onDealDamage)
        {
            var aura = await _pool.Get<BaseSaveThrowEffect>(ct);
            aura.transform.position = center;
            var main = aura.ParticleSystem.main;
            if (damageType == null)
                main.startColor = Color.magenta;
            else 
                main.startColor = new ParticleSystem.MinMaxGradient(damageType.Color);
            
            aura.gameObject.SetActive(true);
            aura.ParticleSystem.Play();
            
            await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
            
            await onDealDamage.Invoke();

            await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
            
            aura.ParticleSystem.Stop();

            await UniTask.WaitForSeconds(1.5f, cancellationToken: ct);
            
            aura.gameObject.SetActive(false);
            _pool.Return(aura);
        }
    }
}