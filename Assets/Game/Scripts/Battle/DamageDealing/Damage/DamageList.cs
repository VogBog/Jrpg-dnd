using System.Collections.Generic;
using JetBrains.Annotations;

namespace Game.Scripts.Battle.DamageDealing.Damage
{
    public struct DamageList
    {
        public List<DamageValue> Values;

        [CanBeNull]
        public DamageType GetMainDamageType()
        {
            if (Values.Count < 1)
                return null;

            return Values[0].DamageType;
        }

        public DamageList(List<DamageValue> values)
        {
            Values = values;
        }
    }
}