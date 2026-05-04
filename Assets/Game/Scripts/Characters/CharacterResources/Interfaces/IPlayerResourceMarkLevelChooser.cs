using System;
using Game.Scripts.Characters.CharacterResources.Data;

namespace Game.Scripts.Characters.CharacterResources.Interfaces
{
    public interface IPlayerResourceMarkLevelChooser : ICharacterResourceMarkLevelChooser
    {
        event Action<ChooseMarkLevelCommand> Opened;
        event Action Closed;
    }
}