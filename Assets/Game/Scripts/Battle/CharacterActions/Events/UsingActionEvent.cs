using System.Collections.Generic;
using Game.Scripts.Battle.CharacterActions.Interfaces;

namespace Game.Scripts.Battle.CharacterActions.Events
{
    public class UsingActionEvent
    {
        public ICharacterAction Action { get; private set; }
        public IBattleUnit BattleUnit { get; private set; }
        public List<IBattleUnit> Targets { get; private set; }
        public bool Cancel { get; set; }

        public void SetData(ICharacterAction action, IBattleUnit battleUnit, List<IBattleUnit> targets)
        {
            Action = action;
            BattleUnit = battleUnit;
            Targets = targets;
        }

        public void Clear()
        {
            Action = null;
            BattleUnit = null;
            Cancel = false;
        }
    }
}