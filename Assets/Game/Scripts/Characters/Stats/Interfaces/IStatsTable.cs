namespace Game.Scripts.Characters.Stats.Interfaces
{
    public interface IStatsTable
    {
        int Level { get; }
        int MasteryBonus { get; }
        Data.Stats SpellStat { get; }
        
        int GetValue(Data.Stats stat);
        int GetModifier(Data.Stats stat);
        int GetSaveThrow(Data.Stats stat);
        bool HasSaveThrow(Data.Stats stat);
    }
}