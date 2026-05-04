using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Events.Data
{
    public class AsyncQueryableSubscription<T>
    {
        public readonly Func<T, CancellationToken, UniTask> Handler;
        public Predicate<T> Filter;
        public EventStep Step;
        
        private readonly Action<AsyncQueryableSubscription<T>, EventStep, EventStep> _stepChanged;

        public AsyncQueryableSubscription(
            Func<T, CancellationToken, UniTask> handler,
            Action<AsyncQueryableSubscription<T>, EventStep, EventStep> stepChanged)
        {
            Handler = handler;
            Filter = null;
            Step = EventStep.ChangeValuesAndSetEffects;
            _stepChanged = stepChanged;
        }

        public AsyncQueryableSubscription<T> When(Predicate<T> filter)
        {
            Filter = filter;
            return this;
        }

        public AsyncQueryableSubscription<T> On(EventStep step)
        {
            _stepChanged?.Invoke(this, Step, step);
            Step = step;
            return this;
        }
    }
}