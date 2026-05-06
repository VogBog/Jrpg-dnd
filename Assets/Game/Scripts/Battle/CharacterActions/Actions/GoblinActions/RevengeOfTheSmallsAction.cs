using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.BattleAnimations.Interfaces;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.StatusEffects.Data;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Characters.ModularCharacters;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.GoblinActions
{
    public class RevengeOfTheSmallsAction : BaseCharacterAction
    {
        [Inject] private IInitiativeQueue _queue;
        [Inject] private IBattleAnimationsPlayer _animations;

        [SerializeField] private RevengeOfTheSmallsStatusEffect _statusEffect;
        
        public override bool CanUse()
        {
            if (!base.CanUse())
                return false;

            int goblinsCount = 0;
            foreach (var data in _queue.Queue)
            {
                if (UnitTeamsUtils.GetTeam(data.Unit) is UnitTeams.Enemy)
                    goblinsCount++;
            }

            return goblinsCount > 3;
        }

        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            return CanUse();
        }

        protected override async UniTask OnUse(CancellationToken ct)
        {
            if (!Owner.GameObject.TryGetComponent(out IStatusEffectProcessor statusEffectProcessor))
                return;

            var command = new AttachStatusEffectCommand(Owner, _statusEffect, EndEffectTimes.TurnEnd, 1);

            await _animations.PlayMagicUseAsync(
                Owner,
                Owner.UnitContainer.TryResolve<IBattleUnitAnimator>(),
                Owner.GameObject.transform,
                async _ => statusEffectProcessor.Attach(command),
                ct);
        }

        protected override void ClearDataAfterUse()
        {
            
        }
        
        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { Owner };
    }
}