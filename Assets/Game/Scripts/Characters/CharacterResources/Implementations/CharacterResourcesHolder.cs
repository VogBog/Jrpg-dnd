using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Battle;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Characters.CharacterResources.DefaultResources;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using Game.Scripts.Events.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Characters.CharacterResources.Implementations
{
    public class CharacterResourcesHolder : MonoBehaviour, ICharacterResourcesHolder
    {
        [Inject] private IBattleUnit _ownerUnit;
        [Inject] private IEventBus _eventBus;
        
        private readonly Dictionary<CharacterResourceData, ICharacterResource> _resources = new();
        private readonly Dictionary<(CharacterResourceData, int), ICharacterMarkedResource> _marked = new();
        private readonly EmptyResource _emptyResource = new();

        public event Action<IEnumerable<ICharacterResource>, int> ResourcesListChanged;
        public event Action<ICharacterResource> SingleResourceChanged;
        
        public ICharacterResource AddNew(CharacterResourceData resource)
        {
            if (_resources.ContainsKey(resource))
                throw new Exception($"CharacterResourcesHolder.AddNew: Resource already exists: {resource}");

            var newResource = _ownerUnit.UnitContainer.Instantiate<BaseResource>();
            newResource.Data = resource;
            _resources.Add(resource, newResource);

            SetEventsAfterAddingNew(newResource);
            return newResource;
        }

        public ICharacterMarkedResource AddNewMarked(CharacterMarkedResourceData resource, int mark)
        {
            if (_marked.ContainsKey((resource, mark)))
                throw new Exception($"CharacterResourcesHolder.AddNewMarked: Resource already exists: {resource} {mark}");
            
            var newResource = _ownerUnit.UnitContainer.Instantiate<BaseMarkedResource>();
            newResource.Data = resource;
            newResource.MarkValue = mark;
            newResource.MarkedData = resource;
            _marked.Add((resource, mark), newResource);
            
            SetEventsAfterAddingNew(newResource);
            return newResource;
        }

        public ICharacterResource Add(CharacterResourceData resource, int count)
        {
            if (count <= 0)
                throw new ArgumentException("CharacterResourcesHolder.Add: Count must be greater than 0");
            
            if (!_resources.TryGetValue(resource, out var existingResource))
            {
                existingResource = AddNew(resource);
                count--;
            }

            existingResource?.Add(count);
            return existingResource;
        }

        public ICharacterMarkedResource AddMarked(CharacterMarkedResourceData resource, int count, int mark)
        {
            if (count <= 0)
                throw new ArgumentException("CharacterResourcesHolder.AddMarked: Count must be greater than 0");

            if (!_marked.TryGetValue((resource, mark), out var existingResource))
            {
                existingResource = AddNewMarked(resource, mark);
                count--;
            }
            
            existingResource?.Add(count);
            return existingResource;
        }

        public bool RemoveAll(CharacterResourceData resource)
        {
            if (_resources.TryGetValue(resource, out var existingResource))
            {
                existingResource.Changed -= OnResourceChanged;
                _resources.Remove(resource);
                OnListChanged();
                return true;
            }

            var toRemove = new List<(CharacterResourceData, int)>();
            foreach (var kvp in _marked)
            {
                if (kvp.Key.Item1 == resource)
                {
                    toRemove.Add(kvp.Key);
                    kvp.Value.Changed -= OnResourceChanged;
                }
            }

            if (toRemove.Count > 0)
            {
                foreach (var remove in toRemove)
                    _marked.Remove(remove);

                OnListChanged();
                return true;
            }

            return false;
        }

        public bool RemoveAllMarked(CharacterResourceData resource, int mark)
        {
            if (_marked.TryGetValue((resource, mark), out var existingResource))
            {
                existingResource.Changed -= OnResourceChanged;
                _marked.Remove((resource, mark));
                OnListChanged();
                return true;
            }

            return false;
        }

        public bool RemoveSome(CharacterResourceData resource, int count)
        {
            if (count < 0)
                throw new ArgumentException("CharacterResourcesHolder.Remove: Count must be equals or greater than 0");
            
            if (_resources.TryGetValue(resource, out var existingResource))
            {
                existingResource.Remove(count);
                if (existingResource.MaxCount == 0)
                {
                    _resources.Remove(resource);
                    existingResource.Changed -= OnResourceChanged;
                    OnListChanged();
                }
                    
                return true;
            }

            return false;
        }

        public bool RemoveSomeMarked(CharacterResourceData resource, int count, int mark)
        {
            if (count < 0)
                throw new ArgumentException("CharacterResourcesHolder.Remove: Count must be equals or greater than 0");

            if (_marked.TryGetValue((resource, mark), out var existingResource))
            {
                existingResource.Remove(count);
                if (existingResource.MaxCount == 0)
                {
                    _marked.Remove((resource, mark));
                    existingResource.Changed -= OnResourceChanged;
                    OnListChanged();
                }

                return true;
            }

            return false;
        }

        public ICharacterResource Get(CharacterResourceData resource)
        {
            return _resources[resource] ?? _emptyResource;
        }

        public ICharacterMarkedResource GetMarked(CharacterResourceData resource, int mark, bool getHigherIfNotFound = false)
        {
            var result = _marked[(resource, mark)];
            if (result != null)
                return result;

            foreach (var kvp in _marked)
            {
                if (kvp.Key.Item1 == resource &&
                    kvp.Key.Item2 >= mark)
                {
                    return kvp.Value;
                }
            }

            return _emptyResource;
        }

        public int GetMaxMarkLevelOf(CharacterResourceData resource, bool thatCanUse)
        {
            int maxNum = int.MinValue;
            foreach (var kvp in _marked)
            {
                if (kvp.Key.Item1 == resource &&
                    !(thatCanUse && !kvp.Value.CanUse()) &&
                    kvp.Key.Item2 > maxNum)
                {
                    maxNum = kvp.Key.Item2;
                }
            }

            return maxNum;
        }

        public void Use(CharacterResourceData resource)
        {
            if (_resources.TryGetValue(resource, out var existingResource))
            {
                existingResource.Use();
            } 
        }

        public void UseMarked(CharacterResourceData resource, int mark)
        {
            if (_marked.TryGetValue((resource, mark), out var existingResource))
            {
                existingResource.Use();
            }
        }

        public IEnumerable<ICharacterResource> GetAll()
        {
            return _resources.Values.Concat(_marked.Values);
        }

        public IEnumerable<ICharacterMarkedResource> GetAllMarkedBy(CharacterResourceData resource)
        {
            return _marked
                .Where(x => x.Key.Item1 == resource)
                .Select(x => x.Value)
                .OfType<ICharacterMarkedResource>();
        }

        public void RestoreForNewTurn()
        {
            foreach (var resource in _resources.Values)
            {
                if (resource.Data.IsRestoreInNextTurn)
                    resource.Restore();
            }

            foreach (var resource in _marked.Values)
            {
                if (resource.Data.IsRestoreInNextTurn)
                    resource.Restore();
            }
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<TurnStartingEvent>(OnTurnStarted)
                .When(x => x.UnitData.Unit == _ownerUnit);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<TurnStartingEvent>(OnTurnStarted);
        }

        private void OnTurnStarted(TurnStartingEvent ev)
        {
            RestoreForNewTurn();
        }

        private void OnResourceChanged(ICharacterResource resource)
        {
            SingleResourceChanged?.Invoke(resource);
        }

        private void SetEventsAfterAddingNew(ICharacterResource newResource)
        {
            newResource.Changed += OnResourceChanged;
            OnListChanged();
        }

        private void OnListChanged()
        {
            ResourcesListChanged?.Invoke(GetAll(), _resources.Count + _marked.Count);
        }
    }
}