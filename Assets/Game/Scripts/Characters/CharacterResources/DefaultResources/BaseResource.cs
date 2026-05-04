using System;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using UnityEngine;

namespace Game.Scripts.Characters.CharacterResources.DefaultResources
{
    public class BaseResource : ICharacterResource
    {
        public CharacterResourceData Data { get; set; }
        
        public int Count { get; protected set; } = 1;
        public int MaxCount { get; protected set; } = 1;
        public bool IsRestoreInNextTurn { get; private set; }
        
        public event Action<ICharacterResource> Changed;

        public bool CanUse() => Count > 0;

        public void Restore() => Count = MaxCount;

        public void Restore(int count) => Count = Mathf.Clamp(Count + count, 0, MaxCount);

        public void Use()
        {
            if (Count == 0)
                throw new InvalidOperationException($"BaseResource.Use: Cannot use an empty resource.");

            Count--;
            Changed?.Invoke(this);
        }

        public void Add(int count)
        {
            if (count < 0)
                throw new InvalidOperationException($"BaseResource.Add({count}): count cannot be negative.");
            
            Count += count;
            MaxCount += count;
            Changed?.Invoke(this);
        }

        public void Remove(int count)
        {
            if (count < 0)
                throw new InvalidOperationException($"BaseResource.Remove({count}): count cannot be negative.");
            
            MaxCount = Mathf.Clamp(MaxCount - count, 0, MaxCount);
            Count = Mathf.Clamp(Count - count, 0, MaxCount);
            Changed?.Invoke(this);
        }
    }
}