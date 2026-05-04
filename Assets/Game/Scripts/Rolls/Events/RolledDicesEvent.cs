using System.Collections.Generic;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Events
{
    public class RolledDicesEvent
    {
        public RollDiceParameters Parameters;
        public int Result;
        public bool MustReRoll;
        public List<RolledDicesResult> Rolls;

        public RolledDicesEvent(
            RollDiceParameters parameters,
            int result,
            List<RolledDicesResult> rolls)
        {
            Parameters = parameters;
            Result = result;
            MustReRoll = false;
            Rolls = rolls;
        }
        
        

        public struct RolledDicesResult
        {
            public List<RolledSingleDiceResult> Rolls;

            public RolledDicesResult(List<RolledSingleDiceResult> rolls)
            {
                Rolls = rolls;
            }
        }

        public struct RolledSingleDiceResult
        {
            public int Dice;
            public int Result;

            public bool MustReRoll;

            public RolledSingleDiceResult(int dice, int result)
            {
                Dice = dice;
                Result = result;
                MustReRoll = false;
            }
        }
    }
}