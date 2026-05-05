using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Events.Data;
using Game.Scripts.Events.Interfaces;

namespace Game.Scripts.Events.Implementations
{
    public class AsyncQueryableObservable<T> : IAsyncQueryableEvent<T>, IAsyncObservable<T> where T : class
    {
        private readonly List<AsyncQueryableSubscription<T>> _subscriptions = new();

        public AsyncQueryableSubscription<T> Subscribe(Func<T, CancellationToken, UniTask> taskBuilder)
        {
            var subscription = new AsyncQueryableSubscription<T>(taskBuilder, OnStepChanged);
            return Subscribe(subscription);
        }

        public void Unsubscribe(Func<T, CancellationToken, UniTask> taskBuilder)
        {
            int index = _subscriptions.FindIndex(x => x.Handler == taskBuilder);
            if (index != -1)
            {
                _subscriptions.RemoveAt(index);
            }
        }

        public void Unsubscribe(AsyncQueryableSubscription<T> subscription)
        {
            _subscriptions.Remove(subscription);
        }

        public async UniTask Invoke(T ev, CancellationToken ct)
        {
            var copy = new List<AsyncQueryableSubscription<T>>(_subscriptions);
            foreach (var subscription in copy)
            {
                if (subscription.Filter == null || subscription.Filter.Invoke(ev))
                    await subscription.Handler.Invoke(ev, ct);
            }
        }

        public void Clear()
        {
            _subscriptions.Clear();
        }
        
        private AsyncQueryableSubscription<T> Subscribe(AsyncQueryableSubscription<T> subscription)
        {
            for (int i = 0; i < _subscriptions.Count; i++)
            {
                if (subscription.Step > _subscriptions[i].Step)
                {
                    _subscriptions.Insert(i + 1, subscription);
                    return subscription;
                }
            }
            
            _subscriptions.Add(subscription);
            return subscription;
        }

        private void OnStepChanged(AsyncQueryableSubscription<T> subscription, int fromStep, int toStep)
        {
            Unsubscribe(subscription);
            Subscribe(subscription);
        }
    }
}