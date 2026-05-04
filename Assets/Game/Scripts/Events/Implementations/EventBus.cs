using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Events.Data;
using Game.Scripts.Events.Interfaces;

namespace Game.Scripts.Events.Implementations
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, object> _asyncObservables = new();
        private readonly Dictionary<Type, object> _syncObservables = new();
        
        public AsyncQueryableSubscription<T> Subscribe<T>(Func<T, CancellationToken, UniTask> handler) where T : class
        {
            var type = typeof(T);
            if (!_asyncObservables.TryGetValue(type, out var observableObj))
            {
                observableObj = new AsyncQueryableObservable<T>();
                _asyncObservables[type] = observableObj;
            }
            
            var observable = (AsyncQueryableObservable<T>)observableObj;
            return observable.Subscribe(handler);
        }

        public void Unsubscribe<T>(Func<T, CancellationToken, UniTask> handler) where T : class
        {
            var type = typeof(T);
            if (!_asyncObservables.TryGetValue(type, out var observableObj))
                return;
            
            var observable = (AsyncQueryableObservable<T>)observableObj;
            observable.Unsubscribe(handler);
        }

        public void Unsubscribe<T>(AsyncQueryableSubscription<T> subscription) where T : class
        {
            var type = typeof(T);
            if (!_asyncObservables.TryGetValue(type, out var observableObj))
                return;
            
            var observable = (AsyncQueryableObservable<T>)observableObj;
            observable.Unsubscribe(subscription);
        }

        public UniTask Publish<T>(T message, CancellationToken cancellationToken) where T : class
        {
            if (_syncObservables.TryGetValue(typeof(T), out var so) &&
                so is QueryableObservable<T> syncObservable)
                syncObservable.Invoke(message);
            
            if (!_asyncObservables.TryGetValue(typeof(T), out var observableObj))
                return UniTask.CompletedTask;
            
            var observable = (AsyncQueryableObservable<T>)observableObj;
            return observable.Invoke(message, cancellationToken);
        }

        public QueryableSubscription<T> Subscribe<T>(Action<T> handler) where T : class
        {
            var type = typeof(T);
            if (!_syncObservables.TryGetValue(type, out var observableObj))
            {
                observableObj = new QueryableObservable<T>();
                _syncObservables[type] = observableObj;
            }
            
            var observable = (QueryableObservable<T>)observableObj;
            return observable.Subscribe(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            if (!_syncObservables.TryGetValue(typeof(T), out var observableObj))
                return;
            
            var observable = (QueryableObservable<T>)observableObj;
            observable.Unsubscribe(handler);
        }

        public void Unsubscribe<T>(QueryableSubscription<T> subscription) where T : class
        {
            if (!_syncObservables.TryGetValue(typeof(T), out var observableObj))
                return;
            
            var observable = (QueryableObservable<T>)observableObj;
            observable.Unsubscribe(subscription);
        }

        public void Publish<T>(T message) where T : class
        {
            if (!_syncObservables.TryGetValue(typeof(T), out var observableObj))
                return;
            
            var observable = (QueryableObservable<T>)observableObj;
            observable.Invoke(message);
        }
    }
}