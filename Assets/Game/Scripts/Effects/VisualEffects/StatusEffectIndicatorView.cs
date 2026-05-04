using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Effects.VisualEffects
{
    public class StatusEffectIndicatorView : MonoBehaviour
    {
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public TMP_Text Name { get; private set; }
        [field: SerializeField] public TMP_Text Underline { get; private set; }
    }
}