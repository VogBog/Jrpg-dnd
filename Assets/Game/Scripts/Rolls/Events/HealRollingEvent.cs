using Game.Scripts.Battle;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Events
{
    public class HealRollingEvent
    {
        public IBattleUnit Target { get; private set; }
        public IBattleUnit Caster { get; private set; }
        public RollDiceParameters Parameters { get; private set; }
        public bool Cancel;

        public HealRollingEvent Init(
            IBattleUnit target,
            IBattleUnit caster,
            RollDiceParameters parameters)
        {
            Target = target;
            Caster = caster;
            Parameters = parameters;
            Cancel = false;

            return this;
        }
    }
}