using Game.Scripts.Battle.StatusEffects.Interfaces;
using UnityEngine;

namespace Game.Scripts.Battle.StatusEffects.Data
{
    public struct TempEffectData
    {
        public readonly AttachStatusEffectCommand Command;
        public readonly Object EffectObject;
        public readonly IStatusEffect Effect;
        public readonly EndEffectTimes EndEffectTime;
        public int Rounds;
        public bool HasDuplicate;
        public bool IsDuplicate;

        public TempEffectData(
            AttachStatusEffectCommand command,
            Object effectObject,
            IStatusEffect effect,
            EndEffectTimes endEffectTime,
            int rounds,
            bool isDuplicate)
        {
            Command = command;
            EffectObject = effectObject;
            Effect = effect;
            EndEffectTime = endEffectTime;
            Rounds = rounds;
            HasDuplicate = false;
            IsDuplicate = isDuplicate;
        }
    }
}