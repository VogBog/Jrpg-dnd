using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Scripts.UI.CharacterAnalyze
{
    public class CharacterAnalyzeDamageReactionViewItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public TMP_Text Text { get; private set; }
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _infoText;

        [HideInInspector] public string InfoText;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _panel.SetActive(true);
            _infoText.text = InfoText;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _panel.SetActive(false);
        }
    }
}