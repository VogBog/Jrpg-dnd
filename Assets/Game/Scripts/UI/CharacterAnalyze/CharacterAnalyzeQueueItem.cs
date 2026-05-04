using Game.Scripts.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.CharacterAnalyze
{
    public class CharacterAnalyzeQueueItem : MonoBehaviour
    {
        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }

        public IBattleUnit Target;
    }
}