using System.Collections.Generic;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Characters.CharacterResources.DefaultResources;

namespace Game.Scripts.Battle.CharacterActions.Interfaces
{
    public interface ICharacterAction
    {
        IBattleUnit Owner { get; }
        
        IEnumerable<CharacterResourceData> UseResources { get; }
        IEnumerable<MarkedResourceRequirementSerializable> UseMarkedResources { get; }

        bool CanUse();
    }
}