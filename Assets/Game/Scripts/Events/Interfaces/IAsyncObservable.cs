using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Events.Interfaces
{
    public interface IAsyncObservable<in T>
    {
        UniTask Invoke(T ev, CancellationToken ct);
        void Clear();
    }
}