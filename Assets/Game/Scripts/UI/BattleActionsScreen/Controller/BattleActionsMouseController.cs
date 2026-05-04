using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Scenes;
using Game.Scripts.UI.BattleActionsScreen.ModelView;
using Game.Scripts.UI.BattleActionsScreen.View;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.BattleActionsScreen.Controller
{
    [RequireComponent(typeof(BattleActionsView))]
    public class BattleActionsMouseController : MonoBehaviour
    {
        private BattleActionsView _view;
        private IInitiativeQueue _initiativeQueue;
        private IAsyncOperationScope _scope;
        private BattleActionsModelView _mw;

        private ICharacterAction _hoveredAction;

        [Inject]
        private void Construct(IInitiativeQueue initiativeQueue, IAsyncOperationScope scope)
        {
            _initiativeQueue = initiativeQueue;
            _scope = scope;
            _view = GetComponent<BattleActionsView>();
            _mw = GetComponent<BattleActionsModelView>();
        }

        private void OnEnable()
        {
            _view.ActionItemInstantiated += OnNewItemInstantiated;
            _view.PageItemInstantiated += OnNewPageItemInstantiated;
        }

        private void OnDisable()
        {
            _view.ActionItemInstantiated -= OnNewItemInstantiated;
            _view.PageItemInstantiated -= OnNewPageItemInstantiated;
        }

        private void OnNewItemInstantiated(BattleActionItemView item)
        {
            item.Clicked += OnActionClicked;
            item.Hovered += OnItemHovered;
            item.HoverEnded += OnItemHoverEnds;
        }

        private void OnNewPageItemInstantiated(BattleActionsViewPageItem pageItem)
        {
            pageItem.Clicked += OnPageClicked;
        }

        private void OnActionClicked(ICharacterAction action)
        {
            var unit = _initiativeQueue.UnitMakingTurn.Unit;
            if (!enabled || unit == null || action == null || !action.CanUse())
                return;

            if (!unit.GameObject.TryGetComponent(out ICharacterActionsHolder actionsHolder) ||
                !actionsHolder.CanUseActions() ||
                action is not IActiveAction activeAction)
                return;

            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(actionsHolder.TryUse(activeAction, ct));
        }

        private void OnItemHovered(ICharacterAction action)
        {
            _hoveredAction = action;
            _mw.ShowActionInfo(action);
        }

        private void OnItemHoverEnds(ICharacterAction action)
        {
            if (_hoveredAction != action)
                return;
            _hoveredAction = null;
            _mw.HideActionInfo();
        }

        private void OnPageClicked(int pageIndex)
        {
            _mw?.SelectPage(pageIndex);
        }
    }
}