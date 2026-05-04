namespace Game.Scripts.Battle.TargetChooser.Data
{
    public readonly struct ChooseTargetCommand
    {
        public readonly TargetFilter Filter;
        public readonly int MaxCount;

        public ChooseTargetCommand(TargetFilter filter, int maxCount)
        {
            Filter = filter;
            MaxCount = maxCount;
        }
    }
}