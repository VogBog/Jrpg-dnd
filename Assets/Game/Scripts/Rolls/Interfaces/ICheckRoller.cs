using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Battle.DamageDealing.Weapon;
using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Interfaces
{
    public interface ICheckRoller
    {
        UniTask<RollResult> InitiativeCheck(
            IBattleUnit unit,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder = null);
        
        UniTask<bool> SaveThrow(
            IBattleUnit target,
            IBattleUnit attacker,
            Stats stat,
            int difficulty,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder = null);
        
        UniTask<AttackRollResults> AttackRoll(
            IBattleUnit target,
            WeaponHolder attacker,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder = null);
        
        UniTask<AttackRollResults> AttackRollNotWeapon(
            IBattleUnit target,
            IBattleUnit attacker,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder = null);

        UniTask<RolledDamageList> DamageRoll(
            IBattleUnit target,
            IBattleUnit attacker,
            DamageList damageList,
            CancellationToken ct);

        UniTask<int> HealRoll(
            IBattleUnit target,
            IBattleUnit caster,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder);
    }
}