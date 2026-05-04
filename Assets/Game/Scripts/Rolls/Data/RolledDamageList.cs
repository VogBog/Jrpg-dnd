using System.Collections.Generic;
using Game.Scripts.Battle.DamageDealing.Damage;
using JetBrains.Annotations;

namespace Game.Scripts.Rolls.Data
{
    public struct RolledDamageList
    {
        public readonly List<RolledDamageValue> Values;

        public RolledDamageList(List<RolledDamageValue> values)
        {
            Values = values;
        }
        
        [CanBeNull]
        public DamageType GetMainDamageType()
        {
            if (Values.Count < 1)
                return null;

            return Values[0].DamageType;
        }
    }
}