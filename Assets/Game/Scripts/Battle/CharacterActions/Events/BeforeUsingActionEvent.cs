using Game.Scripts.Battle.CharacterActions.Interfaces;

namespace Game.Scripts.Battle.CharacterActions.Events
{
    public class BeforeUsingActionEvent
    {
        public IBattleUnit BattleUnit { get; private set; }
        public ICharacterAction Action { get; private set; }
        public bool Cancel { get; set; }

        public void SetData(IBattleUnit battleUnit, ICharacterAction action)
        {
            BattleUnit = battleUnit;
            Action = action;
            Cancel = false;
        }

        public void Clear()
        {
            BattleUnit = null;
            Action = null;
            Cancel = false;
        }
    }
}