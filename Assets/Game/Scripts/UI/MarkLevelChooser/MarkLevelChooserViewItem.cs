using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.MarkLevelChooser
{
    public class MarkLevelChooserViewItem : MonoBehaviour
    {
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }

        [HideInInspector] public int MarkLevel;

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
            Clicked?.Invoke(MarkLevel);
        }
    }
}