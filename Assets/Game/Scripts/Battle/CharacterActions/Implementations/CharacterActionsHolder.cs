using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Battle.CharacterActions.Implementations
{
    [RequireComponent(typeof(IBattleUnit))]
    public class CharacterActionsHolder : MonoBehaviour, ICharacterActionsHolder
    {
        private readonly Dictionary<Object, (ICharacterAction, Object)> _actions = new();
        private bool _usingAction = false;
        private IBattleUnit _ownerUnit;

        public event Action<ICharacterActionsHolder> ActionsListChanged;
        
        [Inject]
        private void Construct(IBattleUnit battleUnit)
        {
            _ownerUnit = battleUnit;
        }

        public IEnumerable<ICharacterAction> GetAllActions() => _actions.Values.Select(x => x.Item1);
        
        public void Add(Object prefab)
        {
            if (_actions.ContainsKey(prefab))
                return;

            var instance = _ownerUnit.UnitContainer.InstantiatePrefab(prefab);
            if (!instance.TryGetComponent(out ICharacterAction characterAction))
                throw new NullReferenceException(
                    $"CharacterActionsHolder.Add: Cannot find ICharacterAction on {prefab}");
            
            _actions.Add(prefab, (characterAction, instance));
            if (characterAction is IPassiveEffect passiveAction)
                passiveAction.OnAdded(_ownerUnit);
            
            ActionsListChanged?.Invoke(this);
        }

        public bool Remove(Object prefab)
        {
            if (_actions.TryGetValue(prefab, out var value))
            {
                if (value.Item1 is IPassiveEffect passiveAction)
                    passiveAction.OnRemoved();
                
                Destroy(value.Item2);
                _actions.Remove(prefab);
                ActionsListChanged?.Invoke(this);
                return true;
            }

            return false;
        }

        public ICharacterAction Get(Object prefab)
        {
            if (_actions.TryGetValue(prefab, out var value))
                return value.Item1;
            return null;
        }

        public bool CanUseActions() => !_usingAction && _ownerUnit.IsMyTurn;

        public async UniTask<bool> TryUse(Object prefab, CancellationToken ct)
        {
            if (!_actions.TryGetValue(prefab, out var value) ||
                value.Item1 is not IActiveAction activeAction)
                return false;

            return await TryUse(activeAction, ct);
        }

        public async UniTask<bool> TryUse(IActiveAction action, CancellationToken ct)
        {
            if (action.Owner != _ownerUnit)
                throw new Exception($"CharacterActionsHolder.TryUse: cannot use {action} with other owner. " +
                                    $"Owner of action is {action.Owner} while owner of holder is {_ownerUnit}");
            
            if (!CanUseActions() || !action.CanUse())
                return false;

            bool success = false;
            try
            {
                _usingAction = true;
                success = await action.Use(ct);
            }
            finally
            {
                _usingAction = false;
            }

            return success;
        }
    }
}