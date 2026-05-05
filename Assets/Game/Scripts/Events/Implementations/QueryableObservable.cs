using System;
using System.Collections.Generic;
using Game.Scripts.Events.Data;

namespace Game.Scripts.Events.Implementations
{
    public class QueryableObservable<T>
    {
        private readonly List<QueryableSubscription<T>> _subscriptions = new();

        public QueryableSubscription<T> Subscribe(Action<T> handler)
        {
            var subscription = new QueryableSubscription<T>(handler, OnStepChanged);
            return Subscribe(subscription);
        }

        public void Unsubscribe(Action<T> handler)
        {
            int index = _subscriptions.FindIndex(x => x.Handler == handler);
            if (index != -1)
            {
                _subscriptions.RemoveAt(index);
            }
        }

        public void Unsubscribe(QueryableSubscription<T> subscription)
        {
            _subscriptions.Remove(subscription);
        }

        public void Invoke(T message)
        {
            var copy = new List<QueryableSubscription<T>>(_subscriptions);
            foreach (var subscription in copy)
            {
                if (subscription.Filter == null || subscription.Filter.Invoke(message))
                    subscription.Handler.Invoke(message);
            }
        }

        private QueryableSubscription<T> Subscribe(QueryableSubscription<T> subscription)
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

        private void OnStepChanged(QueryableSubscription<T> subscription, int fromStep, int toStep)
        {
            Unsubscribe(subscription);
            Subscribe(subscription);
        }
    }
}