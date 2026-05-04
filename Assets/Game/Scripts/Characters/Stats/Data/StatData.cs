using System;

namespace Game.Scripts.Characters.Stats.Data
{
    [Serializable]
    public struct StatData
    {
        public Stats Stat;
        public int Value;
        public bool SaveThrow;
        public int SaveThrowBonus;

        public StatData(Stats stat, int value, bool saveThrow = false, int saveThrowBonus = 0)
        {
            Stat = stat;
            Value = value;
            SaveThrow = saveThrow;
            SaveThrowBonus = saveThrowBonus;
        }
    }
}