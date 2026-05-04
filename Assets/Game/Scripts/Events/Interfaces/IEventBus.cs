using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Events.Data;

namespace Game.Scripts.Events.Interfaces
{
    public interface IEventBus
    {
        AsyncQueryableSubscription<T> Subscribe<T>(Func<T, CancellationToken, UniTask> handler) where T : class;
        void Unsubscribe<T>(Func<T, CancellationToken, UniTask> handler) where T : class;
        void Unsubscribe<T>(AsyncQueryableSubscription<T> subscription) where T : class;
        UniTask Publish<T>(T message, CancellationToken cancellationToken) where T : class;
        
        QueryableSubscription<T> Subscribe<T>(Action<T> handler) where T : class;
        void Unsubscribe<T>(Action<T> handler) where T : class;
        void Unsubscribe<T>(QueryableSubscription<T> subscription) where T : class;
        void Publish<T>(T message) where T : class;
    }
}