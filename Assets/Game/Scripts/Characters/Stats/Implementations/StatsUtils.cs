using Game.Scripts.Characters.Stats.Interfaces;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Characters.Stats.Implementations
{
    public static class StatsUtils
    {
        public static int GetSpellSaveThrowDifficulty(this IStatsTable statsTable)
            => 8 + statsTable.MasteryBonus + statsTable.GetModifier(statsTable.SpellStat);

        public static void SetSpellAttackRollBonuses(this IStatsTable statsTable, RollDiceParameters parameters)
            => parameters.AddBonus(statsTable.MasteryBonus + statsTable.GetModifier(statsTable.SpellStat));

        public static int GetHighestModificator(this IStatsTable statsTable, params Data.Stats[] stats)
        {
            if (stats == null)
                return 0;
            
            int maxMod = int.MinValue;
            foreach (var stat in stats)
            {
                int mod = statsTable.GetModifier(stat);
                if (mod > maxMod)
                    maxMod = mod;
            }
            
            return maxMod;
        }
    }
}