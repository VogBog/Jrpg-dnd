using Game.Scripts.Battle;
using Game.Scripts.Battle.DamageDealing.Damage;

namespace Game.Scripts.Rolls.Events
{
    public class DamageRollingEvent
    {
        public IBattleUnit Attacker { get; private set; }
        public IBattleUnit Target { get; private set; }
        
        public DamageList DamageList;

        public DamageRollingEvent Init(IBattleUnit attacker, IBattleUnit target, DamageList damageList)
        {
            Attacker = attacker;
            Target = target;
            DamageList = damageList;

            return this;
        }
    }
}