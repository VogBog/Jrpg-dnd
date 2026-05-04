using Game.Scripts.Battle.CharacterActions.Interfaces;

namespace Game.Scripts.Battle.CharacterActions.Events
{
    public class ActionUsedEvent
    {
        public IBattleUnit BattleUnit { get; private set; }
        public ICharacterAction Action { get; private set; }

        public void SetData(IBattleUnit battleUnit, ICharacterAction action)
        {
            BattleUnit = battleUnit;
            Action = action;
        }

        public void Clear()
        {
            BattleUnit = null;
            Action = null;
        }
    }
}