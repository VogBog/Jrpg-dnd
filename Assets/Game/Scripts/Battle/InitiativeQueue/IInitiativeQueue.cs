using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Battle.InitiativeQueue
{
    public interface IInitiativeQueue
    {
        InitiativeUnitData UnitMakingTurn { get; }
        IEnumerable<InitiativeUnitData> Queue { get; }
        
        event Action<InitiativeQueueChangedEvent> UnitAdded; 
        event Action<InitiativeQueueChangedEvent> UnitRemoved;
        event Action<InitiativeUnitData> NextTurnStarted; 
        
        UniTask AddUnit(IBattleUnit unit, CancellationToken ct);
        void AddUnitInTheEnd(IBattleUnit unit);
        UniTask RemoveUnitAsync(IBattleUnit unit, CancellationToken ct);
        void EndTurn();
        void StartFirstTurn();
    }
}