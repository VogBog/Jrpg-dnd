using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Events;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.FighterActions
{
    public class ChampionCritsPassive : BaseCharacterAction, IPassiveEffect
    {
        [Inject] private IEventBus _bus;
        
        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            return true;
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
            _bus.Subscribe<AttackRollingEvent>(OnAttackRolling).When(x => x.Attacker == Owner);
        }

        public void OnRemoved()
        {
            _bus.Unsubscribe<AttackRollingEvent>(OnAttackRolling);
        }

        private void OnAttackRolling(AttackRollingEvent ev)
        {
            ev.Parameters.AddCritAvailability(1);
        }
        
        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { Owner };
    }
}