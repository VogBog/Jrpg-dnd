using System.Collections.Generic;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using JetBrains.Annotations;

namespace Game.Scripts.Battle.CharacterActions.Events
{
    public class UsingSpellEvent
    {
        public IBattleUnit Caster { get; private set; }
        public ICharacterResourcesHolder ResourcesHolder { get; private set; }
        public ICharacterActionsHolder ActionsHolder { get; private set; }
        public ICharacterAction Action { get; private set; }

        [CanBeNull] public List<IBattleUnit> Targets;
        public bool Cancel;

        public UsingSpellEvent SetData(
            IBattleUnit caster,
            ICharacterAction action,
            ICharacterResourcesHolder resourcesHolder,
            ICharacterActionsHolder actionsHolder)
        {
            Caster = caster;
            Action = action;
            ResourcesHolder = resourcesHolder;
            ActionsHolder = actionsHolder;

            Targets = null;
            Cancel = false;

            return this;
        }

        public UsingSpellEvent SetTargets(List<IBattleUnit> targets)
        {
            Targets = targets;
            return this;
        }

        public UsingSpellEvent SetCancel()
        {
            Cancel = true;
            return this;
        }

        public void Clear()
        {
            Caster = null;
            ResourcesHolder = null;
            ActionsHolder = null;
            Action = null;
            Targets = null;
        }
    }
}