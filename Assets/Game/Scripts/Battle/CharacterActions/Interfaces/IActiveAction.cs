using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Battle.CharacterActions.Interfaces
{
    public interface IActiveAction : ICharacterAction
    {
        bool InUse { get; }
        UniTask<bool> Use(CancellationToken ct);
    }
}