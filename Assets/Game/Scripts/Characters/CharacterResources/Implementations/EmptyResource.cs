using System;
using Game.Scripts.Characters.CharacterResources.DefaultResources;
using Game.Scripts.Characters.CharacterResources.Interfaces;

namespace Game.Scripts.Characters.CharacterResources.Implementations
{
    public class EmptyResource : ICharacterMarkedResource
    {
        public CharacterResourceData Data { get; set; }
        public CharacterMarkedResourceData MarkedData { get; set; }
        public int MarkValue { get; set; }
        
        public int Count => 0;
        public int MaxCount => 0;
        public event Action<ICharacterResource> Changed;
        public bool IsRestoreInNextTurn => false;

        public bool CanUse() => false;

        public void Restore()
        {
        }

        public void Restore(int count)
        {
        }

        public void Use()
        {
        }

        public void Add(int count)
        {
        }

        public void Remove(int count)
        {
        }
    }
}