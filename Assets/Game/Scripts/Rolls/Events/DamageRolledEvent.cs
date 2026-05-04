using Game.Scripts.Battle;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Events
{
    public class DamageRolledEvent
    {
        public IBattleUnit Attacker { get; private set; }
        public IBattleUnit Target { get; private set; }

        public RolledDamageList DamageList;

        public DamageRolledEvent Init(IBattleUnit attacker, IBattleUnit target, RolledDamageList damageList)
        {
            Attacker = attacker;
            Target = target;
            DamageList = damageList;

            return this;
        }
    }
}