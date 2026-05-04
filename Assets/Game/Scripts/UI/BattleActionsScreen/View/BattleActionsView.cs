using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Scenes;
using Game.Scripts.UI.BattleActionsScreen.ModelView;
using Game.Scripts.UI.Helpers;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.BattleActionsScreen.View
{
    [RequireComponent(typeof(BattleActionsModelView))]
    public class BattleActionsView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Transform _buttonsParent;
        [SerializeField] private TMP_Text _hasNoActionsText;
        [SerializeField] private UIObjectsPool<BattleActionItemView> _pool;
        [SerializeField] private UIObjectsPool<BattleActionsViewPageItem> _pagesPool;
        [SerializeField] private ShowActionInfoPanel _showActionInfoPanel;

        private BattleActionsModelView _mw;
        private IAsyncOperationScope _scope;
        private CancellationTokenSource _cts;

        public event Action<BattleActionsViewPageItem> PageItemInstantiated;
        public event Action<BattleActionItemView> ActionItemInstantiated; 

        [Inject]
        private void Construct(IAsyncOperationScope scope)
        {
            _mw = GetComponent<BattleActionsModelView>();
            _scope = scope;
        }

        private void Awake()
        {
            _panel.SetActive(false);
            _mw.Opened += Open;
            _mw.Closed += Close;
            _mw.ActionsChanged += SetActions;
            _mw.PagesChanged += SetPages;
            _mw.ActionInfoShowing += ShowActionInfo;
            _mw.ActionInfoHided += HideActionInfo;
            _pool.InstantiatedOne += OnNewItemViewInstantiated;
            _pagesPool.InstantiatedOne += OnNewPageItemInstantiated;

            if (_scope.TryGetToken(out var ct))
            {
                _scope.FireAndForget(_pool.InitializeAsync(ct));
                _scope.FireAndForget(_pagesPool.InitializeAsync(ct));
            }
        }

        private void OnDestroy()
        {
            _mw.Opened -= Open;
            _mw.Closed -= Close;
            _mw.ActionsChanged -= SetActions;
            _mw.PagesChanged -= SetPages;
            _mw.ActionInfoShowing -= ShowActionInfo;
            _mw.ActionInfoHided -= HideActionInfo;
            _pool.Dispose();
            _pagesPool.Dispose();
        }

        private void Open()
        {
            _panel.SetActive(true);
            _hasNoActionsText.gameObject.SetActive(_pool.UsingItems.Count == 0);
        }

        private void Close()
        {
            _panel.SetActive(false);
            _pool.ReturnAll();
            _pagesPool.ReturnAll();
        }

        private void SetActions(IEnumerable<ICharacterAction> actions)
        {
            if (_scope.TryGetToken(out var token))
                _scope.FireAndForget(SetActionsAsync(actions, token, false));
        }

        private void SetPages(IEnumerable<string> pages)
        {
            if (_scope.TryGetToken(out var token))
                _scope.FireAndForget(SetPagesAsync(pages, token));
        }

        private void OnNewItemViewInstantiated(BattleActionItemView item)
        {
            ActionItemInstantiated?.Invoke(item);
        }

        private void OnNewPageItemInstantiated(BattleActionsViewPageItem pageItem)
        {
            PageItemInstantiated?.Invoke(pageItem);
        }

        private async UniTask SetActionsAsync(
            IEnumerable<ICharacterAction> actions,
            CancellationToken ct,
            bool reloading)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
            _pool.ReturnAll();
            
            foreach (var action in actions)
            {
                var item = await _pool.GetAsync(ct);
                item.TargetAction = action;
                item.Image.sprite = null;

                if (action is ICharacterActionData data)
                {
                    item.Image.sprite = data.Icon;
                    if (data.Icon == null && !reloading)
                        ReloadActionsAsync(actions, data, ct).Forget();
                }
                
                item.gameObject.SetActive(true);

                if (ct.IsCancellationRequested)
                    return;
                
                _hasNoActionsText.gameObject.SetActive(false);
            }
        }

        private async UniTask ReloadActionsAsync(
            IEnumerable<ICharacterAction> actions,
            ICharacterActionData problemData,
            CancellationToken ct)
        {
            var ct2 = _cts.Token;
            for (int i = 0; i < 10 && problemData.Icon == null; i++)
            {
                await UniTask.WaitForSeconds(0.25f);
                if (ct.IsCancellationRequested || ct2.IsCancellationRequested)
                    return;
            }

            await SetActionsAsync(actions, ct, true);
        }

        private async UniTask SetPagesAsync(IEnumerable<string> pages, CancellationToken ct)
        {
            _pagesPool.ReturnAll();
            int index = 0;
            foreach (var page in pages)
            {
                var item = await _pagesPool.GetAsync(ct);
                item.Index = index;
                item.Text.text = page;
                item.gameObject.SetActive(true);

                ++index;
            }
        }

        private void ShowActionInfo(ActionInfo info)
        {
            _showActionInfoPanel.SetData(info);
            _showActionInfoPanel.gameObject.SetActive(true);
        }

        private void HideActionInfo()
        {
            _showActionInfoPanel.gameObject.SetActive(false);
        }
    }
}