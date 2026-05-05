using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle
{
    public interface IBattleUnit
    {
        string Name { get; }
        bool IsMyTurn { get; }
        Sprite Icon { get; set; }
        
        DiContainer UnitContainer { get; }
        GameObject GameObject { get; }
        UniTask OnEndTurnAsync(CancellationToken ct);
        UniTask OnStartTurnAsync(CancellationToken ct);
    }
}