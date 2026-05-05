using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.PopupWindow.Data;
using Game.Scripts.Battle.PopupWindow.Interfaces;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.FighterActions
{
    public class ProtectionFightingStyle : BaseCharacterAction, IPassiveEffect
    {
        [Inject] private IEventBus _bus;
        [Inject] private IPopupWindowCreator _popupWindow;
        
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
                    ev.RollingEvent.ChangeTarget(Owner);
                }
            }
        }
        
        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { Owner };
    }
}