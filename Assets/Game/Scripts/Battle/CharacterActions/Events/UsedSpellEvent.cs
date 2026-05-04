using System.Collections.Generic;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using JetBrains.Annotations;

namespace Game.Scripts.Battle.CharacterActions.Events
{
    public class UsedSpellEvent
    {
        public IBattleUnit Caster { get; private set; }
        public ICharacterResourcesHolder ResourcesHolder { get; private set; }
        public ICharacterActionsHolder ActionsHolder { get; private set; }
        public ICharacterAction Action { get; private set; }

        [CanBeNull] public List<IBattleUnit> Targets;

        public UsedSpellEvent SetData(
            IBattleUnit caster,
            ICharacterResourcesHolder resourcesHolder,
            ICharacterActionsHolder actionsHolder,
            ICharacterAction action,
            [CanBeNull] List<IBattleUnit> targets)
        {
            Caster = caster;
            ResourcesHolder = resourcesHolder;
            ActionsHolder = actionsHolder;
            Action = action;
            Targets = targets;

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