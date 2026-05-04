using System;
using System.Collections.Generic;

namespace Game.Scripts.Battle.DamageDealing
{
    public class SimpleAttribute
    {
        private readonly List<SimpleStatModifier> _modifiers = new();

        public int Value { get; private set; } = 0;

        public event Action<SimpleAttribute> Changed;

        public void AddModifier(SimpleStatModifier modifier)
        {
            _modifiers.Add(modifier);
            RecalculateValue();
        }

        public void RemoveModifier(SimpleStatModifier modifier)
        {
            _modifiers.Remove(modifier);
            RecalculateValue();
        }

        public void RemoveModifier(string name)
        {
            int index = _modifiers.FindIndex(x => x.UniqName == name);
            if (index == -1)
                return;
            
            _modifiers.RemoveAt(index);
            RecalculateValue();
        }

        private void RecalculateValue()
        {
            int result = 0;
            foreach (var value in _modifiers)
                result += value.AddValue;
            
            Value = result;
            Changed?.Invoke(this);
        }
    }
}