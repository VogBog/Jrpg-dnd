using Game.Scripts.Battle;
using Game.Scripts.Battle.DamageDealing.Weapon;
using Game.Scripts.Rolls.Data;
using JetBrains.Annotations;

namespace Game.Scripts.Rolls.Events
{
    public class AttackRollingEvent
    {
        public IBattleUnit Target { get; private set; }
        public IBattleUnit Attacker { get; private set; }
        public RollDiceParameters Parameters { get; private set; }
        [CanBeNull] public WeaponHolder AttackByWeapon { get; private set; }
        public bool Cancelled;

        public AttackRollingEvent Init(
            IBattleUnit target,
            IBattleUnit attacker,
            RollDiceParameters parameters,
            WeaponHolder attackByWeapon)
        {
            Target = target;
            Attacker = attacker;
            Parameters = parameters;
            AttackByWeapon = attackByWeapon;
            Cancelled = false;

            return this;
        }

        public AttackRollingEvent ChangeTarget(IBattleUnit target)
        {
            Target = target;
            
            return this;
        }
    }
}