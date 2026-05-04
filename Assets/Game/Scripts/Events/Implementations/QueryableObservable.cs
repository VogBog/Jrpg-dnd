using System;
using System.Collections.Generic;
using Game.Scripts.Events.Data;

namespace Game.Scripts.Events.Implementations
{
    public class QueryableObservable<T>
    {
        private readonly List<QueryableSubscription<T>>[] _subscriptions = 
            new List<QueryableSubscription<T>>[Enum.GetValues(typeof(EventStep)).Length];

        public QueryableObservable()
        {
            for (int i = 0; i < _subscriptions.Length; i++)
            {
                _subscriptions[i] = new List<QueryableSubscription<T>>();
            }
        }

        public QueryableSubscription<T> Subscribe(Action<T> handler)
        {
            var subscription = new QueryableSubscription<T>(handler, OnStepChanged);
            _subscriptions[0].Add(subscription);
            return subscription;
        }

        public void Unsubscribe(Action<T> handler)
        {
            for (int i = 0; i < _subscriptions.Length; i++)
            {
                int index = _subscriptions[i].FindIndex(x => x.Handler == handler);
                if (index != -1)
                {
                    _subscriptions[i].RemoveAt(index);
                    break;
                }
            }
        }

        public void Unsubscribe(QueryableSubscription<T> subscription)
        {
            _subscriptions[(int)subscription.Step].Remove(subscription);
        }

        public void Invoke(T message)
        {
            for (int i = 0; i < _subscriptions.Length; i++)
            {
                var copy = new List<QueryableSubscription<T>>(_subscriptions[i]);
                foreach (var subscription in copy)
                {
                    if (subscription.Filter == null || subscription.Filter.Invoke(message))
                        subscription.Handler.Invoke(message);
                }
            }
        }

        private void OnStepChanged(QueryableSubscription<T> subscription, EventStep fromStep, EventStep toStep)
        {
            _subscriptions[(int)fromStep].Remove(subscription);
            _subscriptions[(int)toStep].Add(subscription);
        }
    }
}