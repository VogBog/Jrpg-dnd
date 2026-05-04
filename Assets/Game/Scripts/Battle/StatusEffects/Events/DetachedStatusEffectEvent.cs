using Game.Scripts.Battle.StatusEffects.Interfaces;
using UnityEngine;

namespace Game.Scripts.Battle.StatusEffects.Events
{
    public class DetachedStatusEffectEvent
    {
        public IBattleUnit Target { get; private set; }
        public IStatusEffect Effect { get; private set; }

        public DetachedStatusEffectEvent Init(IBattleUnit target, IStatusEffect effect)
        {
            Target = target;
            Effect = effect;

            return this;
        }
    }
}