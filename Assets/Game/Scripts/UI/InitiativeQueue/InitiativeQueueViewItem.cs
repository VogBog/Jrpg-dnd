using Game.Scripts.Battle.InitiativeQueue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.InitiativeQueue
{
    public class InitiativeQueueViewItem : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Name { get; private set; }
        [field: SerializeField] public Image InitiativeMarker { get; private set; }
        [field: SerializeField] public Image InitiativeRollPanel { get; private set; }
        [field: SerializeField] public TMP_Text InitiativeRollText { get; private set; }

        public InitiativeUnitData Target;
    }
}