using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.BattleAnimations.Interfaces;
using Game.Scripts.Battle.CharacterActions.Actions.DefaultActions;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.DamageDealing.Weapon;
using Game.Scripts.Battle.PopupWindow.Data;
using Game.Scripts.Battle.PopupWindow.Interfaces;
using Game.Scripts.Characters.ModularCharacters;
using Game.Scripts.Events.Data;
using Game.Scripts.Rolls.Events;
using Game.Scripts.Rolls.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.GoblinActions
{
    public class CounterAttackAction : BaseCharacterAction, IPassiveEffect
    {
        [Inject] private IPopupWindowCreator _popupWindow;
        [Inject] private IBattleAnimationsPlayer _animations;
        [Inject] private ICheckRoller _checkRoller;
        
        private IBattleUnit _target;
        
        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            return CanUse();
        }

        protected override UniTask OnUse(CancellationToken ct)
        {
            Debug.Log("OnUse");
            if (!Owner.GameObject.TryGetComponent(out WeaponHolder weaponHolder))
                return UniTask.CompletedTask;

            Debug.Log("PerformAttack");
            return AttackAction.PerformAttackWithAnimation(
                Owner,
                _target,
                weaponHolder,
                _animations,
                Owner.UnitContainer.TryResolve<IBattleUnitAnimator>(),
                _checkRoller,
                ct,
                newTarget => _target = newTarget);
        }

        protected override void ClearDataAfterUse()
        {
            
        }

        protected override List<IBattleUnit> GetTargets()
        {
            return new List<IBattleUnit>
            {
                _target
            };
        }

        public void OnAdded(IBattleUnit owner)
        {
            EventBus.Subscribe<AttackPerformingEvent>(OnAttackMissed)
                .When(x => 
                    x.RolledEvent.RollingEvent.Target == Owner &&
                    !x.Results.Success)
                .On(EventStep.SeeResults);
        }

        public void OnRemoved()
        {
            EventBus.Unsubscribe<AttackPerformingEvent>(OnAttackMissed);
        }

        private async UniTask OnAttackMissed(AttackPerformingEvent ev, CancellationToken ct)
        {
            if (!Owner.GameObject.TryGetComponent(out ICharacterActionsHolder holder))
                return;
            
            var options = new List<PopupWindowOption>
            {
                new("Attack")
            };
            var (success, _) = await 
                _popupWindow.OpenAsync(options, Icon, "Use Counterattack to attack your opponent?", ct);
            
            if (!success)
                return;
            
            _target = ev.RolledEvent.RollingEvent.Attacker;
            await holder.UseForce(this, ct);
            _target = null;
        }
    }
}