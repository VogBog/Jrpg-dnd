using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Characters.CharacterResources.Interfaces;

namespace Game.Scripts.Battle.CharacterActions.Events
{
    public class BeforeUsingSpellEvent
    {
        public IBattleUnit Caster { get; private set; }
        public ICharacterResourcesHolder ResourcesHolder { get; private set; }
        public ICharacterActionsHolder ActionsHolder { get; private set; }
        public ICharacterAction Action { get; private set; }
        public bool Cancel;

        public BeforeUsingSpellEvent SetData(
            IBattleUnit caster,
            ICharacterAction action,
            ICharacterResourcesHolder resourcesHolder,
            ICharacterActionsHolder actionsHolder)
        {
            Caster = caster;
            Action = action;
            ResourcesHolder = resourcesHolder;
            ActionsHolder = actionsHolder;
            
            Cancel = false;

            return this;
        }
    }
}