using System;

namespace Game.Scripts.Events.Data
{
    public class QueryableSubscription<T>
    {
        public readonly Action<T> Handler;
        public Predicate<T> Filter;
        public  EventStep Step;
        
        private readonly Action<QueryableSubscription<T>, EventStep, EventStep> _stepChanged;

        public QueryableSubscription(
            Action<T> handler,
            Action<QueryableSubscription<T>, EventStep, EventStep> stepChanged)
        {
            Handler = handler;
            Filter = null;
            Step = EventStep.ChangeValuesAndSetEffects;
            _stepChanged = stepChanged;
        }

        public QueryableSubscription<T> When(Predicate<T> filter)
        {
            Filter = filter;
            return this;
        }

        public QueryableSubscription<T> On(EventStep step)
        {
            _stepChanged?.Invoke(this, Step, step);
            Step = step;
            return this;
        }
    }
}