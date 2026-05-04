using System;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Battle.StatusEffects.Data;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Battle.CharacterActions.Spells.Templates
{
    [Serializable]
    public struct TemplateActions
    {
        public bool DealDamage;
        public bool HalfIfSuccessSaveThrow;
        public DamageListSerializable Damage;

        public bool SetStatusEffect;
        public AssetReferenceObject<IPassiveEffect> StatusEffect;
        public EndEffectTimes StatusEffectEndTime;
        public int StatusEffectDuration;

        public bool Heal;
        public DiceParameters HealParameters;
    }
}