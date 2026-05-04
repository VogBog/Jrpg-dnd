using System;
using System.Collections.Generic;
using Game.Scripts.Events.Interfaces;

namespace Game.Scripts.Events.Implementations
{
    public class EventPool : IEventPool
    {
        private readonly Dictionary<Type, object> _events = new(16);
        
        public T Get<T>() where T : class
        {
            var result = Get(typeof(T));
            return (T)result;
        }

        public object Get(Type type)
        {
            if (!_events.TryGetValue(type, out var result))
                _events.Add(type, null);

            result = Activator.CreateInstance(type);
            return result;
        }

        public void Return(object value)
        {
            var type = value.GetType();
            _events[type] = value;
        }
    }
}