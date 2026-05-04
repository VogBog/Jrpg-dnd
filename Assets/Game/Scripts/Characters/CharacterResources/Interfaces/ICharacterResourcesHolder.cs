using System;
using System.Collections.Generic;
using Game.Scripts.Characters.CharacterResources.DefaultResources;

namespace Game.Scripts.Characters.CharacterResources.Interfaces
{
    public interface ICharacterResourcesHolder
    {
        event Action<IEnumerable<ICharacterResource>, int> ResourcesListChanged;
        event Action<ICharacterResource> SingleResourceChanged;

        ICharacterResource AddNew(CharacterResourceData resource);
        
        ICharacterMarkedResource AddNewMarked(CharacterMarkedResourceData resource, int mark);
        ICharacterResource Add(CharacterResourceData resource, int count);
        
        ICharacterMarkedResource AddMarked(CharacterMarkedResourceData resource, int count, int mark);
        bool RemoveAll(CharacterResourceData resource);
        bool RemoveAllMarked(CharacterResourceData resource, int mark);
        bool RemoveSome(CharacterResourceData resource, int count);
        bool RemoveSomeMarked(CharacterResourceData resource, int count, int mark);
        ICharacterResource Get(CharacterResourceData resource);
        ICharacterMarkedResource GetMarked(CharacterResourceData resource, int mark, bool getHigherIfNotFound = false);
        int GetMaxMarkLevelOf(CharacterResourceData resource, bool thatCanUse);
        void Use(CharacterResourceData resource);
        void UseMarked(CharacterResourceData resource, int mark);
        IEnumerable<ICharacterResource> GetAll();
        IEnumerable<ICharacterMarkedResource> GetAllMarkedBy(CharacterResourceData resource);
        void RestoreForNewTurn();
    }
}