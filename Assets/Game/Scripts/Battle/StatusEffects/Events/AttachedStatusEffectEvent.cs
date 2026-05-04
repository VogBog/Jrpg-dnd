using Game.Scripts.Battle.StatusEffects.Data;
using Game.Scripts.Battle.StatusEffects.Interfaces;

namespace Game.Scripts.Battle.StatusEffects.Events
{
    public class AttachedStatusEffectEvent
    {
        public AttachStatusEffectCommand Command { get; private set; }
        public IBattleUnit Target { get; private set; }
        public IStatusEffect Effect { get; private set; }

        public AttachedStatusEffectEvent Init(
            AttachStatusEffectCommand command,
            IBattleUnit target,
            IStatusEffect effect)
        {
            Command = command;
            Target = target;
            Effect = effect;

            return this;
        }
    }
}