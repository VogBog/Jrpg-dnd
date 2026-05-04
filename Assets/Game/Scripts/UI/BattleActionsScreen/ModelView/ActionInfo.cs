using System.Collections.Generic;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Characters.CharacterResources.DefaultResources;

namespace Game.Scripts.UI.BattleActionsScreen.ModelView
{
    public struct ActionInfo
    {
        public ICharacterActionData Data;
        public IEnumerable<CharacterResourceData> UsingResources;
        public IEnumerable<(CharacterMarkedResourceData, int)> UsingMarkedResources;

        public ActionInfo(
            ICharacterActionData data,
            IEnumerable<CharacterResourceData> usingResources,
            IEnumerable<(CharacterMarkedResourceData, int)> usingMarkedResources)
        {
            Data = data;
            UsingResources = usingResources;
            UsingMarkedResources = usingMarkedResources;
        }
    }
}