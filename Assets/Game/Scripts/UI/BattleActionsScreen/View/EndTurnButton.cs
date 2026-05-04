using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Scenes;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.BattleActionsScreen.View
{
    [RequireComponent(typeof(Button))]
    public class EndTurnButton : MonoBehaviour
    {
        private Button _button;
        private IInitiativeQueue _initiativeQueue;
        private IAsyncOperationScope _scope;

        [Inject]
        private void Construct(IInitiativeQueue initiativeQueue, IAsyncOperationScope scope)
        {
            _initiativeQueue = initiativeQueue;
            _scope = scope;
            _button = GetComponent<Button>();
        }

        private void Awake()
        {
            _initiativeQueue.NextTurnStarted += OnNextTurnStarted;
            _button.onClick.AddListener(Clicked);
        }

        private void OnDestroy()
        {
            _initiativeQueue.NextTurnStarted -= OnNextTurnStarted;
            _button.onClick.RemoveListener(Clicked);
        }

        private void OnNextTurnStarted(InitiativeUnitData data)
        {
            bool active = UnitTeamsUtils.IsUnderPlayerControl(data.Unit);
            _button.gameObject.SetActive(active);
            _button.enabled = active;
        }

        private void Clicked()
        {
            _button.enabled = false;
            _initiativeQueue.EndTurn();
        }
    }
}