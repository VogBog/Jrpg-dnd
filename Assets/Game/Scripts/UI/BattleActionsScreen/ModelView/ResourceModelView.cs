using Game.Scripts.Characters.CharacterResources.DefaultResources;
using JetBrains.Annotations;

namespace Game.Scripts.UI.BattleActionsScreen.ModelView
{
    public struct ResourceModelView
    {
        public readonly CharacterResourceData ViewData;
        [CanBeNull] public readonly CharacterMarkedResourceData MarkedViewData;
        public int MarkValue;
        public int Count;
        public int MaxCount;

        public ResourceModelView(
            CharacterResourceData viewData,
            int count,
            int maxCount,
            CharacterMarkedResourceData markedData = null,
            int markValue = 0)
        {
            ViewData = viewData;
            Count = count;
            MaxCount = maxCount;
            
            MarkedViewData = markedData;
            MarkValue = markValue;
        }
    }
}