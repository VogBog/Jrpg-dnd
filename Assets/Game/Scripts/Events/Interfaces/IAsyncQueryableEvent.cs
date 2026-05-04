using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Events.Data;

namespace Game.Scripts.Events.Interfaces
{
    public interface IAsyncQueryableEvent<T>
    {
        AsyncQueryableSubscription<T> Subscribe(Func<T, CancellationToken, UniTask> taskBuilder);
        void Unsubscribe(Func<T, CancellationToken, UniTask> taskBuilder);
        void Unsubscribe(AsyncQueryableSubscription<T> subscription);
    }
}