using Game.Scripts.Battle;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Events
{
    public class DamageSingleTypeRollingEvent
    {
        public IBattleUnit Attacker { get; private set; }
        public IBattleUnit Target { get; private set; }
        
        public RollDiceParameters Parameters { get; private set; }

        public DamageSingleTypeRollingEvent Init(
            IBattleUnit attacker,
            IBattleUnit target,
            RollDiceParameters parameters)
        {
            Attacker = attacker;
            Target = target;
            Parameters = parameters;

            return this;
        }
    }
}