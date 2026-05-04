using Game.Scripts.Characters.Stats.Interfaces;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Battle.InitiativeQueue
{
    public class InitiativeRollingEvent
    {
        public IBattleUnit Unit { get; private set; }
        public IStatsTable StatsTable { get; private set; }
        public RollDiceParameters Parameters { get; private set; }

        public InitiativeRollingEvent Init(
            IBattleUnit battleUnit,
            IStatsTable statsTable,
            RollDiceParameters parameters)
        {
            Unit = battleUnit;
            StatsTable = statsTable;
            Parameters = parameters;

            return this;
        }
    }
}