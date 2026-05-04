using Game.Scripts.Battle;
using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Characters.Stats.Interfaces;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Events
{
    public class SaveThrowRollingEvent
    {
        public IBattleUnit Target { get; private set; }
        public IBattleUnit Attacker { get; private set; }
        
        public IStatsTable TargetStats { get; private set; }
        public RollDiceParameters Parameters { get; private set; }
        public Stats ThrowingStat { get; private set; }
        
        public int Difficulty;
        public bool Cancelled;

        public SaveThrowRollingEvent Init(
            IBattleUnit target,
            IBattleUnit attacker,
            IStatsTable statsTable,
            RollDiceParameters parameters,
            Stats throwingStat,
            int difficulty)
        {
            Target = target;
            Attacker = attacker;
            TargetStats = statsTable;
            Parameters = parameters;
            ThrowingStat = throwingStat;
            Difficulty = difficulty;
            Cancelled = false;

            return this;
        }

        public SaveThrowRollingEvent DifficultyUp(int amount)
        {
            Difficulty += amount;
            return this;
        }

        public SaveThrowRollingEvent DifficultyDown(int amount)
        {
            Difficulty -= amount;
            return this;
        }
    }
}