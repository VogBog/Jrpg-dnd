using System;

namespace Game.Scripts.Events.Data
{
    public class QueryableSubscription<T>
    {
        public readonly Action<T> Handler;
        public Predicate<T> Filter;
        public int Step;
        
        private readonly Action<QueryableSubscription<T>, int, int> _stepChanged;

        public QueryableSubscription(
            Action<T> handler,
            Action<QueryableSubscription<T>, int, int> stepChanged)
        {
            Handler = handler;
            Filter = null;
            Step = (int)EventStep.ChangeValuesAndSetEffects;
            _stepChanged = stepChanged;
        }

        public QueryableSubscription<T> When(Predicate<T> filter)
        {
            Filter = filter;
            return this;
        }

        public QueryableSubscription<T> On(EventStep step) => On((int)step);

        public QueryableSubscription<T> On(int step)
        {
            _stepChanged?.Invoke(this, Step, step);
            Step = step;
            return this;
        }
    }
}