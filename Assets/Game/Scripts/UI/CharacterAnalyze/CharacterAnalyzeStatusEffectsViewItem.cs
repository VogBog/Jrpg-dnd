using System;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Scripts.UI.CharacterAnalyze
{
    public class CharacterAnalyzeStatusEffectsViewItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [field: SerializeField] public Image Icon { get; private set; }

        [HideInInspector] public IStatusEffect Target;

        public event Action<IStatusEffect> PointerEntered;
        public event Action<IStatusEffect> PointerExited;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEntered?.Invoke(Target);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExited?.Invoke(Target);
        }
    }
}