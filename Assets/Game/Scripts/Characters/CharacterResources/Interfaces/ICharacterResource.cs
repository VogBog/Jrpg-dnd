using System;
using Game.Scripts.Characters.CharacterResources.DefaultResources;

namespace Game.Scripts.Characters.CharacterResources.Interfaces
{
    public interface ICharacterResource
    {
        CharacterResourceData Data { get; set; }
        
        int Count { get; }
        int MaxCount { get; }

        event Action<ICharacterResource> Changed; 
        
        bool CanUse();
        void Restore();
        void Restore(int count);
        void Use();
        void Add(int count);
        void Remove(int count);
    }
}