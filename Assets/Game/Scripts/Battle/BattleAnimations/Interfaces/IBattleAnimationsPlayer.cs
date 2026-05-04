using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Characters.ModularCharacters;
using UnityEngine;

namespace Game.Scripts.Battle.BattleAnimations.Interfaces
{
    public interface IBattleAnimationsPlayer
    {
        void PlayCloseCombat(
            IBattleUnit attacker,
            IBattleUnitAnimator attackerAnimator,
            IBattleUnit defender,
            Func<CancellationToken, UniTask> dealDamageEvent);
        
        UniTask PlayCloseCombatAsync(
            IBattleUnit attacker,
            IBattleUnitAnimator attackerAnimator,
            IBattleUnit defender,
            Func<CancellationToken, UniTask> dealDamageEvent,
            CancellationToken ct);

        UniTask PlayMagicUseAsync(
            IBattleUnit user,
            IBattleUnitAnimator animator,
            Transform target,
            Func<CancellationToken, UniTask> actionEvent,
            CancellationToken ct);
    }
}