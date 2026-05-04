using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.CharacterAnalyze
{
    public class CharacterAnalyzeStatView : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text NameText { get; private set; }
        [field: SerializeField] public TMP_Text ValueText { get; private set; }
        [field: SerializeField] public TMP_Text ModifierText { get; private set; }
        [field: SerializeField] public TMP_Text SaveThrowText { get; private set; }
        [field: SerializeField] public Image HaveSaveThrow { get; private set; }
    }
}