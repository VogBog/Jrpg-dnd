using System;

namespace Game.Scripts.Battle.CharacterAnalyze.Interfaces
{
    public interface ICharacterSelector
    {
        event Action<IBattleUnit> Selected; 
        
        IBattleUnit SelectedUnit { get; }
        
        void Select(IBattleUnit unit);
    }
}