using Game.Scripts.Battle.CharacterActions.Interfaces;
using UnityEngine;

namespace Game.Scripts.Battle.StatusEffects.Interfaces
{
    public interface IStatusEffect : IPassiveEffect
    {
        public Sprite Icon { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsActive { get; }

        void SetProcessor(IStatusEffectProcessor processor);
        void Activate();
        void Deactivate();
    }
}