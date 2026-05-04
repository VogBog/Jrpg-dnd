using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.BattleActionsScreen.View
{
    public class BattleActionsViewPageItem : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Text { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }

        [HideInInspector] public int Index;

        public event Action<int> Clicked;

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
            Clicked?.Invoke(Index);
        }
    }
}