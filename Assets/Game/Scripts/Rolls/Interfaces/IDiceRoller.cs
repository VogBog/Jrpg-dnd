using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Rolls.Data;

namespace Game.Scripts.Rolls.Interfaces
{
    public interface IDiceRoller
    {
        UniTask<RollResult> Roll(RollDiceParameters parameters, CancellationToken ct);
    }
}