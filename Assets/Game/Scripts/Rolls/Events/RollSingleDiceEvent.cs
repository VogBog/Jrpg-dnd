namespace Game.Scripts.Rolls.Events
{
    public ref struct RollSingleDiceEvent
    {
        public readonly int Dice;
        public int Result;
        
        public delegate void RollSingleDiceDelegate(ref RollSingleDiceEvent e);

        public RollSingleDiceEvent(int dice, int result)
        {
            Dice = dice;
            Result = result;
        }
    }
}