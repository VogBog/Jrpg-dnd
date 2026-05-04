using System;
using System.Collections.Generic;
using Game.Scripts.Battle.CharacterActions.Interfaces;

namespace Game.Scripts.UI.BattleActionsScreen.ModelView
{
    public readonly struct BattleActionsPage
    {
        public readonly string Name;
        public readonly Predicate<ICharacterAction> Filter;

        public BattleActionsPage(string name, Predicate<ICharacterAction> filter)
        {
            Name = name;
            Filter = filter;
        }

        public IEnumerable<ICharacterAction> FilterActions(IEnumerable<ICharacterAction> actions)
        {
            foreach (var action in actions)
            {
                if (Filter.Invoke(action))
                    yield return action;
            }
        }
    }
}