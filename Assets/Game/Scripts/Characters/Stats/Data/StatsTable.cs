using System;
using System.Linq;
using Game.Scripts.Characters.Stats.Interfaces;
using UnityEngine;

namespace Game.Scripts.Characters.Stats.Data
{
    [Serializable]
    public class StatsTableData : IStatsTable
    {
        [SerializeField] private StatData[] _stats;
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public int MasteryBonus { get; private set; }
        [field: SerializeField] public Stats SpellStat { get; private set; }

        public int GetValue(Stats stat)
        {
            return _stats.FirstOrDefault(x => x.Stat == stat).Value;
        }

        public int GetModifier(Stats stat)
        {
            var value = _stats.FirstOrDefault(x => x.Stat == stat).Value;
            return ValueToModifier(value);
        }

        public int GetSaveThrow(Stats stat)
        {
            var data = _stats.FirstOrDefault(x => x.Stat == stat);
            int bonus = data.SaveThrowBonus + (data.SaveThrow ? MasteryBonus : 0);
            return ValueToModifier(data.Value) + bonus;
        }

        public bool HasSaveThrow(Stats stat)
        {
            return _stats.FirstOrDefault(x => x.Stat == stat).SaveThrow;
        }

        public static int ValueToModifier(int value) => (value - 10) / 2;

        public void OnValidate()
        {
            var enumArray = Enum.GetValues(typeof(Stats)) as Stats[];
            if (enumArray == null)
                return;
            
            if (_stats.Length != enumArray.Length)
                _stats = new StatData[enumArray.Length];

            for (int i = 0; i < _stats.Length; i++)
            {
                _stats[i].Stat = enumArray[i];
            }
        }
    }
}