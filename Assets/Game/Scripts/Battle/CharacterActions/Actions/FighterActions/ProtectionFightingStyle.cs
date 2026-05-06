using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Helpers.DOTweenExtensions;
using Game.Scripts.Battle.CameraController.Data;
using Game.Scripts.Battle.CameraController.Interfaces;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.PopupWindow.Data;
using Game.Scripts.Battle.PopupWindow.Interfaces;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.FighterActions
{
    public class ProtectionFightingStyle : BaseCharacterAction, IPassiveEffect
    {
        [Inject] private IEventBus _bus;
        [Inject] private IPopupWindowCreator _popupWindow;
        [Inject] private ICameraController _cameraController;
        [Inject] private IUnitBattlePositioning _unitBattlePositioning;

        private ProtectionFightingStyleSwapPositionsCommand _swapPositionsBack;
        
        public const float SwapPositionsTime = 0.5f;
        public const float GoBackDistance = 2f;
        
        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            return CanUse();
        }

        protected override UniTask OnUse(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        protected override void ClearDataAfterUse()
        {
            
        }

        public void OnAdded(IBattleUnit owner)
        {
            _bus.Subscribe<AttackRolledEvent>(OnAttackRolled).When(x => 
                x.RollingEvent.Target != Owner && UnitTeamsUtils.GetTeam(x.RollingEvent.Target) is UnitTeams.Ally &&
                CanUse());
        }

        public void OnRemoved()
        {
            _bus.Unsubscribe<AttackRolledEvent>(OnAttackRolled);
        }

        private async UniTask OnAttackRolled(AttackRolledEvent ev, CancellationToken ct)
        {
            var options = new List<PopupWindowOption>
            {
                new PopupWindowOption("Yes")
            };

            var (success, _) = await _popupWindow.OpenAsync(options, Icon, "Stand in for an ally?", ct);

            if (success)
            {
                bool used = await Use(ct);

                if (used)
                {
                    var follow = _cameraController.Follow;
                    var targets = _cameraController.Targets.ToList();
                    targets.Remove(ev.RollingEvent.Target.GameObject.transform);
                    if (!targets.Contains(Owner.GameObject.transform))
                        targets.Add(Owner.GameObject.transform);

                    _swapPositionsBack = new(
                        ev.RollingEvent.Target.GameObject.transform,
                        _unitBattlePositioning.GetUsedPositionForUnit(
                            UnitTeamsUtils.GetTeamUnit(ev.RollingEvent.Target)) ?? Vector3.zero,
                        _unitBattlePositioning.GetUsedPositionForUnit(
                            UnitTeamsUtils.GetTeamUnit(Owner)) ?? Vector3.zero);

                    _bus.Subscribe<TurnEndingEvent>(SwapPositionsBack);
                    
                    var newPos = ev.RollingEvent.Target.GameObject.transform.position;
                    ev.RollingEvent.Target.GameObject.transform.DOMove(
                        newPos - GoBackDistance * ev.RollingEvent.Target.GameObject.transform.forward,
                        SwapPositionsTime);
                    await Owner.GameObject.transform.DOMove(newPos, SwapPositionsTime).ToUniTask(ct);
                    
                    ev.RollingEvent.ChangeTarget(Owner);
                    _cameraController.SetTargets(follow, targets, false, ZoomType.Far);
                }
            }
        }
        
        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { Owner };

        private async UniTask SwapPositionsBack(TurnEndingEvent ev, CancellationToken ct)
        {
            _bus.Unsubscribe<TurnEndingEvent>(SwapPositionsBack);
            
            _swapPositionsBack.SwapWith.DOMove(_swapPositionsBack.TargetPosition, SwapPositionsTime);
            await Owner.GameObject.transform.DOMove(_swapPositionsBack.MyPosition, SwapPositionsTime)
                .ToUniTask(ct);
        }
    }
}