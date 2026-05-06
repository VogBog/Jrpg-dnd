using System.Collections.Generic;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Characters.Stats.Data;
using UnityEngine;

namespace Game.Scripts.UI.CharacterAnalyze
{
    public readonly struct CharacterAnalyzeRedrawData
    {
        public readonly Sprite Avatar;
        public readonly string Name;
        public readonly int Health;
        public readonly int MaxHealth;
        public readonly int Armor;
        public readonly StatInfo[] Stats;
        public readonly IEnumerable<(DamageType, DamageReactionTypes)> DamageReactions;
        public readonly IEnumerable<IStatusEffect> StatusEffects;

        public readonly bool InfiniteHealth;
        public readonly bool InfiniteArmor;

        public CharacterAnalyzeRedrawData(
            Sprite avatar,
            string name,
            int health,
            int maxHealth,
            int armor,
            StatInfo[] stats,
            IEnumerable<(DamageType, DamageReactionTypes)> damageReactions,
            IEnumerable<IStatusEffect> statusEffects,
            bool infiniteHealth = false,
            bool infiniteArmor = false)
        {
            Avatar = avatar;
            Name = name;
            Health = health;
            MaxHealth = maxHealth;
            Armor = armor;
            Stats = stats;
            DamageReactions = damageReactions;
            StatusEffects = statusEffects;
            InfiniteHealth = infiniteHealth;
            InfiniteArmor = infiniteArmor;
        }



        public readonly struct StatInfo
        {
            public readonly Stats Stat;
            public readonly int Value;
            public readonly int Modifier;
            public readonly int SaveThrow;
            public readonly bool HaveSaveThrow;

            public StatInfo(Stats stat, int value, int modifier, int saveThrow, bool haveSaveThrow)
            {
                Stat = stat;
                Value = value;
                Modifier = modifier;
                SaveThrow = saveThrow;
                HaveSaveThrow = haveSaveThrow;
            }
        }
    }
}