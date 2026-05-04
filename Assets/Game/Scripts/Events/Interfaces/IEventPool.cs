using System;

namespace Game.Scripts.Events.Interfaces
{
    public interface IEventPool
    {
        T Get<T>() where T : class;
        object Get(Type type);
        void Return(object value);
    }
}