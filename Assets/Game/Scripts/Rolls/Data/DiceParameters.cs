using System;

namespace Game.Scripts.Rolls.Data
{
    [Serializable]
    public struct DiceParameters
    {
        public int Count;
        public int Dice;

        public DiceParameters(int count, int dice)
        {
            Count = count;
            Dice = dice;
        }
    }
}