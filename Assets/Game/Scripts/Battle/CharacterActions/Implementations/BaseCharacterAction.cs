using System.Collections.Generic;
using Game.Scripts.Battle.CharacterActions.Events;

namespace Game.Scripts.Battle.CharacterActions.Implementations
{
    public abstract class BaseCharacterAction : 
        BaseAction<BeforeUsingActionEvent, UsingActionEvent, ActionUsedEvent>
    {
        protected override BeforeUsingActionEvent SetBeforeUsingActionData(BeforeUsingActionEvent beforeUseEvent)
        {
            beforeUseEvent.SetData(Owner, this);
            return beforeUseEvent;
        }

        protected override UsingActionEvent SetUsingActionData(UsingActionEvent usingEvent)
        {
            usingEvent.SetData(this, Owner, GetTargets());
            return usingEvent;
        }

        protected override ActionUsedEvent SetUsedActionData(ActionUsedEvent usedEvent)
        {
            usedEvent.SetData(Owner, this);
            return usedEvent;
        }

        protected override bool GetCancelAndClearBeforeUseEvent(BeforeUsingActionEvent beforeUseEvent)
        {
            bool cancelled = beforeUseEvent.Cancel;
            beforeUseEvent.Clear();
            return cancelled;
        }

        protected override bool GetCancelAndClearUsingEvent(UsingActionEvent usingEvent)
        {
            bool cancelled = usingEvent.Cancel;
            usingEvent.Clear();
            return cancelled;
        }

        protected override void ClearUsedEvent(ActionUsedEvent usedEvent)
        {
            usedEvent.Clear();
        }

        protected abstract List<IBattleUnit> GetTargets();
    }
}