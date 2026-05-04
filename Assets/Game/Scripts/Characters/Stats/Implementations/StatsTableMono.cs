using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Characters.Stats.Interfaces;
using UnityEngine;

namespace Game.Scripts.Characters.Stats.Implementations
{
    public class StatsTableMono : MonoBehaviour, IStatsTable
    {
        [SerializeField] private StatsTableData _data;

        public int Level => _data.Level;
        public int MasteryBonus => _data.MasteryBonus;
        public Data.Stats SpellStat => _data.SpellStat;
        
        public int GetValue(Data.Stats stat) => _data.GetValue(stat);

        public int GetModifier(Data.Stats stat) => _data.GetModifier(stat);

        public int GetSaveThrow(Data.Stats stat) => _data.GetSaveThrow(stat);
        
        public bool HasSaveThrow(Data.Stats stat) => _data.HasSaveThrow(stat);

        #if UNITY_EDITOR
        private void OnValidate()
        {
            _data.OnValidate();
        }
        #endif
    }
}