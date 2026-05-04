using System;
using System.Collections.Generic;
using Game.Scripts.Battle.StatusEffects.Data;

namespace Game.Scripts.Battle.StatusEffects.Interfaces
{
    public interface IStatusEffectProcessor
    {
        event Action<IStatusEffectProcessor> Changed; 
        
        void Attach(AttachStatusEffectCommand command);
        void Detach(AttachStatusEffectCommand command);
        void Detach(IStatusEffect effect);
        IEnumerable<IStatusEffect> GetAll(bool onlyActive);
    }
}