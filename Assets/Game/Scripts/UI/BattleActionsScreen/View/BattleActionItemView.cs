using System;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Scripts.UI.BattleActionsScreen.View
{
    public class BattleActionItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }

        public ICharacterAction TargetAction;
        public event Action<ICharacterAction> Clicked;
        public event Action<ICharacterAction> Hovered;
        public event Action<ICharacterAction> HoverEnded; 

        private void OnEnable()
        {
            Button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            Clicked?.Invoke(TargetAction);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hovered?.Invoke(TargetAction);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HoverEnded?.Invoke(TargetAction);
        }
    }
}