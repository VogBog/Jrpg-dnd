using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Game.Scripts.Battle.CharacterActions.Interfaces
{
    public interface ICharacterActionsHolder
    {
        event Action<ICharacterActionsHolder> ActionsListChanged;
        
        IEnumerable<ICharacterAction> GetAllActions();
        
        void Add(Object prefab);
        bool Remove(Object prefab);
        
        [CanBeNull] ICharacterAction Get(Object prefab);

        bool CanUseActions();
        
        UniTask<bool> TryUse(Object prefab, CancellationToken ct);
        UniTask<bool> TryUse(IActiveAction action, CancellationToken ct);
    }
}