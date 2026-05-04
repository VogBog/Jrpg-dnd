using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.BattleAnimations.Interfaces;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;
using Game.Scripts.Characters.ModularCharacters;
using Game.Scripts.Characters.Stats.Interfaces;
using Game.Scripts.Rolls.Interfaces;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.FighterActions
{
    public class SecondWindAction : BaseCharacterAction, IActiveAction
    {
        [Inject] private IBattleAnimationsPlayer _animations;
        [Inject] private ICheckRoller _checkRoller;
        
        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            return CanUse();
        }

        protected override async UniTask OnUse(CancellationToken ct)
        {
            if (!Owner.GameObject.TryGetComponent(out IHealthProcessor healthProcessor))
                return;

            int level = 1;
            if (Owner.GameObject.TryGetComponent(out IStatsTable stats))
                level = stats.Level;

            await _animations.PlayMagicUseAsync(
                Owner,
                Owner.UnitContainer.TryResolve<IBattleUnitAnimator>(),
                Owner.GameObject.transform,
                async _ =>
                {
                    int heal = await _checkRoller.HealRoll(
                        Owner,
                        Owner,
                        ct,
                        param => param.Init(1, 10).AddBonus(level));

                    var healCommand = new HealCommand(Owner, heal);
                    await healthProcessor.Heal(healCommand, ct);
                },
                ct);
        }

        protected override void ClearDataAfterUse()
        {
            
        }
        
        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { Owner };
    }
}