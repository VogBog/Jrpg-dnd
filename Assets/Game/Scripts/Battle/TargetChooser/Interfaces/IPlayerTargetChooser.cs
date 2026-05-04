using System;
using System.Collections.Generic;
using Game.Scripts.Battle.TargetChooser.Data;

namespace Game.Scripts.Battle.TargetChooser.Interfaces
{
    public interface IPlayerTargetChooser : ITargetChooser
    {
        event Action<ChooseTargetCommand> SelectionStarted; 
        event Action<List<IBattleUnit>, IBattleUnit> SelectedUnitsChanged;
        event Action SelectionEnded;

        void Submit();
        void Cancel();
    }
}