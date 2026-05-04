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
        private readonly List<AsyncQueryableSubscription<T>>[] _subscriptions = 
            new List<AsyncQueryableSubscription<T>>[Enum.GetValues(typeof(EventStep)).Length];

        public AsyncQueryableObservable()
        {
            for (int i = 0; i < _subscriptions.Length; i++)
            {
                _subscriptions[i] = new List<AsyncQueryableSubscription<T>>();
            }
        }

        public AsyncQueryableSubscription<T> Subscribe(Func<T, CancellationToken, UniTask> taskBuilder)
        {
            var subscription = new AsyncQueryableSubscription<T>(taskBuilder, OnStepChanged);
            _subscriptions[0].Add(subscription);
            return subscription;
        }

        public void Unsubscribe(Func<T, CancellationToken, UniTask> taskBuilder)
        {
            for (int i = 0; i < _subscriptions.Length; i++)
            {
                int index = _subscriptions[i].FindIndex(x => x.Handler == taskBuilder);
                if (index != -1)
                {
                    _subscriptions[i].RemoveAt(index);
                    break;
                }
            }
        }

        public void Unsubscribe(AsyncQueryableSubscription<T> subscription)
        {
            _subscriptions[(int)subscription.Step].Remove(subscription);
        }

        public async UniTask Invoke(T ev, CancellationToken ct)
        {
            for (int i = 0; i < _subscriptions.Length; i++)
            {
                var copy = new List<AsyncQueryableSubscription<T>>(_subscriptions[i]);
                foreach (var subscription in copy)
                {
                    if (subscription.Filter == null || subscription.Filter.Invoke(ev))
                        await subscription.Handler.Invoke(ev, ct);
                }
            }
        }

        public void Clear()
        {
            foreach (var list in _subscriptions)
            {
                list.Clear();
            }
        }

        private void OnStepChanged(AsyncQueryableSubscription<T> subscription, EventStep fromStep, EventStep toStep)
        {
            _subscriptions[(int)fromStep].Remove(subscription);
            _subscriptions[(int)toStep].Add(subscription);
        }
    }
}