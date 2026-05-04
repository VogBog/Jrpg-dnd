using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Events.Interfaces
{
    public interface IAsyncEvent<out T>
    {
        void Subscribe(Func<T, CancellationToken, UniTask> taskBuilder);
        void Unsubscribe(Func<T, CancellationToken, UniTask> taskBuilder);
    }
}