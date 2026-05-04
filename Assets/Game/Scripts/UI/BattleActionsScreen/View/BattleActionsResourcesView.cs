using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Scenes;
using Game.Scripts.UI.BattleActionsScreen.ModelView;
using Game.Scripts.UI.Helpers;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.BattleActionsScreen.View
{
    [RequireComponent(typeof(BattleResourcesModelView))]
    public class BattleActionsResourcesView : MonoBehaviour
    {
        public const int InstantiateItemsAtStartCount = 4;
        
        [SerializeField] private Transform _parent;
        [SerializeField] private UIObjectsPool<BattleResourcesItemView> _pool;
        
        private BattleResourcesModelView _mw;
        private IAsyncOperationScope _scope;

        [Inject]
        private void Construct(IAsyncOperationScope scope)
        {
            _scope = scope;
            _mw = GetComponent<BattleResourcesModelView>();
        }

        private void Awake()
        {
            _mw.ResourcesListChanged += RepaintAll;
            _mw.SingleResourceChanged += RepaintOne;
            _mw.Closed += _pool.ReturnAll;
            
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.InitializeAsync(ct, InstantiateItemsAtStartCount));
        }
        
        private void OnDestroy()
        {
            _mw.ResourcesListChanged -= RepaintAll;
            _mw.SingleResourceChanged -= RepaintOne;
            _mw.Closed -= _pool.ReturnAll;
            _pool.Dispose();
        }

        private void RepaintAll(IEnumerable<ResourceModelView> resources)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(RepaintAllAsync(resources, ct));
        }

        private async UniTask RepaintAllAsync(IEnumerable<ResourceModelView> resources, CancellationToken ct)
        {
            _pool.ReturnAll();
            foreach (var resource in resources)
            {
                var item = await _pool.GetAsync(ct);
                if (ct.IsCancellationRequested) break;

                PaintOne(resource, item);
                
                item.gameObject.SetActive(true);
            }
        }

        private void RepaintOne(ResourceModelView resource)
        {
            foreach (var item in _pool.UsingItems)
            {
                if (item.Target == resource.ViewData && 
                    (resource.MarkedViewData == null || resource.MarkValue == item.MarkLevel))
                {
                    PaintOne(resource, item);
                    break;
                }
            }
        }

        private void PaintOne(ResourceModelView resource, BattleResourcesItemView item)
        {
            var icon = resource.ViewData.Icon;
            if (resource.MarkedViewData != null)
            {
                item.MarkLevel = resource.MarkValue;
                foreach (var variant in resource.MarkedViewData.Variants)
                {
                    if (variant.MarkValue == resource.MarkValue)
                    {
                        icon = variant.Icon;
                        break;
                    }
                }
            }
            
            item.SetData(icon, resource.Count, resource.MaxCount, resource.ViewData.Color);
            item.Target = resource.ViewData;
        }
    }
}