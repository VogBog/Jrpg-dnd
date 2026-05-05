using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Helpers.DOTweenExtensions;
using Game.Scripts.Battle.BattleAnimations.Interfaces;
using Game.Scripts.Battle.CameraController.Interfaces;
using Game.Scripts.Battle.DamageDealing.Health.Events;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Characters.ModularCharacters;
using Game.Scripts.Events.Data;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using Game.Scripts.Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Battle.BattleAnimations.Implementations
{
    public class BattleAnimationsPlayer : MonoBehaviour, IBattleAnimationsPlayer
    {
        [Inject] private IAsyncOperationScope _scope;
        [Inject] private IInitiativeQueue _queue;
        [Inject] private IEventBus _bus;
        [Inject] private ICameraController _cameraController;

        [SerializeField] private AssetReferenceT<AttackAnimationData> _magicAnimationReference;
        private AttackAnimationData _magicAnimation = null;

        public const float CloseCombatMoveDuration = 0.25f;
        
        public void PlayCloseCombat(
            IBattleUnit attacker,
            IBattleUnitAnimator attackerAnimator,
            IBattleUnit defender,
            Func<CancellationToken, UniTask> dealDamageEvent)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(
                    PlayCloseCombatAsync(
                        attacker, attackerAnimator, defender, dealDamageEvent, ct));
        }

        public async UniTask PlayCloseCombatAsync(
            IBattleUnit attacker,
            IBattleUnitAnimator attackerAnimator,
            IBattleUnit defender,
            Func<CancellationToken, UniTask> dealDamageEvent,
            CancellationToken ct)
        {
            _cameraController.SetTargets(
                defender.GameObject.transform,
                true,
                attacker.GameObject.transform);
            
            var initPos = attacker.GameObject.transform.position;
            var initRot = attacker.GameObject.transform.rotation;
            
            var direction = Vector3.Normalize(
                attacker.GameObject.transform.position - defender.GameObject.transform.position);
            attacker.GameObject.transform.LookAt(attacker.GameObject.transform.position - direction);

            var offset = direction * 2f;
            if (attackerAnimator != null)
            {
                offset = direction * (attackerAnimator.GetAttackData()?.HitDistance ?? 2f);
            }
            
            var combatPos = defender.GameObject.transform.position + offset;

            await attacker.GameObject.transform.DOMove(combatPos, CloseCombatMoveDuration).ToUniTask(ct);
            float time = 1f;
            if (attackerAnimator != null)
            {
                var data = attackerAnimator.Attack();
                time = data.Duration - data.HitTiming;
            
                await UniTask.WaitForSeconds(data.HitTiming, cancellationToken: ct);
            }
            
            await dealDamageEvent.Invoke(ct);
            
            if (time > 0f)
                await UniTask.WaitForSeconds(time, cancellationToken: ct);

            await attacker.GameObject.transform.DOMove(initPos, CloseCombatMoveDuration).ToUniTask(ct);
            attacker.GameObject.transform.rotation = initRot;
            
            _cameraController.ReturnToBack();
        }

        public async UniTask PlayMagicUseAsync(
            IBattleUnit user,
            IBattleUnitAnimator animator,
            Transform target,
            Func< CancellationToken, UniTask> actionEvent,
            CancellationToken ct)
        {
            var initRot = user.GameObject.transform.rotation;

            if (user.GameObject.transform != target)
            {
                user.GameObject.transform.LookAt(target);
            }

            if (animator != null)
            {
                var oldAnimation = animator.GetAttackData();
                if (_magicAnimation == null)
                    _magicAnimation = await _magicAnimationReference.LoadAssetAsync();
                animator.SetAttackId(_magicAnimation);
                animator.Attack();

                await UniTask.WaitForSeconds(_magicAnimation.HitTiming, cancellationToken: ct);

                await actionEvent.Invoke(ct);

                float time = _magicAnimation.Duration - _magicAnimation.HitTiming;
                
                if (time > 0f)
                    await UniTask.WaitForSeconds(time, cancellationToken: ct);
                
                animator.SetAttackId(oldAnimation);
            }
            else
            {
                await UniTask.WaitForSeconds(1f, cancellationToken: ct);
                await actionEvent.Invoke(ct);
                await UniTask.WaitForSeconds(1f, cancellationToken: ct);
            }
            
            user.GameObject.transform.rotation = initRot;
        }

        private void OnEnable()
        {
            _queue.UnitAdded += OnUnitAdded;
            _bus.Subscribe<DiedEvent>(OnUnitDied).On(EventStep.CancelEffects);
            _bus.Subscribe<AttackPerformingEvent>(OnUnitAttacked).On(EventStep.SeeResults);
        }

        private void OnDisable()
        {
            if (_queue != null)
            {
                _queue.UnitAdded -= OnUnitAdded;
            }

            if (_bus != null)
            {
                _bus.Unsubscribe<DiedEvent>(OnUnitDied);
                _bus.Unsubscribe<AttackPerformingEvent>(OnUnitAttacked);
            }
        }

        private void OnUnitAdded(InitiativeQueueChangedEvent ev)
        {
            var unit = ev.ChangedUnit.Unit;
            var team = UnitTeamsUtils.GetTeam(unit);

            IBattleUnit nearestEnemy = null;
            float minDist = float.MaxValue;

            foreach (var unitData in _queue.Queue)
            {
                if (UnitTeamsUtils.GetTeam(unitData.Unit) != team)
                {
                    float dist = Vector3.Distance(
                        unit.GameObject.transform.position, unitData.Unit.GameObject.transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestEnemy = unitData.Unit;
                    }
                }
            }

            if (nearestEnemy == null)
            {
                if (_scope.TryGetToken(out var ct))
                    _scope.FireAndForget(RetryUnitChanged(ev, ct));
                return;
            }
            
            unit.GameObject.transform.LookAt(nearestEnemy.GameObject.transform);
        }

        private async UniTask RetryUnitChanged(InitiativeQueueChangedEvent ev, CancellationToken ct)
        {
            await UniTask.WaitForSeconds(1f, cancellationToken: ct);
            OnUnitAdded(ev);
        }

        private void OnUnitAttacked(AttackPerformingEvent ev)
        {
            if (ev.Results.Success)
                return;
            
            var animator = ev.RolledEvent.RollingEvent.Target.UnitContainer.TryResolve<IBattleUnitAnimator>();
            animator?.Dodge();
        }

        private async UniTask OnUnitDied(DiedEvent ev, CancellationToken ct)
        {
            var animator = ev.Unit.UnitContainer.TryResolve<IBattleUnitAnimator>();
            if (animator != null)
            {
                animator.Die();
                await UniTask.WaitForSeconds(3f, cancellationToken: ct);
            }
        }
    }
}