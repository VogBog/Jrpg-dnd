namespace Game.Scripts.Battle.DamageDealing
{
    public class SimpleStatModifier
    {
        public string UniqName;
        public int AddValue;

        public SimpleStatModifier(string uniqName, int addValue)
        {
            UniqName = uniqName;
            AddValue = addValue;
        }
    }
}