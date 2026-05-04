using UnityEngine;

namespace Game.Scripts.Battle.StatusEffects.Data
{
    public readonly struct AttachStatusEffectCommand
    {
        public readonly IBattleUnit Caster;
        public readonly Object Prefab;
        public readonly EndEffectTimes EndEffectTime;
        public readonly int Rounds;

        public AttachStatusEffectCommand(
            IBattleUnit caster,
            Object effect,
            EndEffectTimes endEffectTime,
            int rounds)
        {
            Caster = caster;
            Prefab = effect;
            EndEffectTime = endEffectTime;
            Rounds = rounds;
        }
    }
}