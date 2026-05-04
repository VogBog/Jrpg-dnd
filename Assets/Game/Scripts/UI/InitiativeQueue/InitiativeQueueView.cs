using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Scenes;
using Game.Scripts.UI.Helpers;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.InitiativeQueue
{
    public class InitiativeQueueView : MonoBehaviour
    {
        public const int CreateItemsAtStartCount = 8;
        
        [SerializeField] private float _itemAnimationDelta;
        [SerializeField] private float _itemAnimationDuration;
        [SerializeField] private float _showInitiativeDuration;
        [SerializeField] private UIObjectsPool<InitiativeQueueViewItem> _pool;
        
        private IAsyncOperationScope _scope;
        private IInitiativeQueue _initiativeQueue;

        [Inject]
        private void Construct(
            IAsyncOperationScope scope,
            IInitiativeQueue initiativeQueue)
        {
            _scope = scope;
            _initiativeQueue = initiativeQueue;
        }

        private void OnEnable()
        {
            _initiativeQueue.UnitAdded += OnUnitAdded;
            _initiativeQueue.UnitRemoved += OnUnitRemoved;
            _initiativeQueue.NextTurnStarted += OnNextTurnStarted;
        }

        private void OnDisable()
        {
            _initiativeQueue.UnitAdded -= OnUnitAdded;
            _initiativeQueue.UnitRemoved -= OnUnitRemoved;
            _initiativeQueue.NextTurnStarted -= OnNextTurnStarted;
        }

        private void OnNextTurnStarted(InitiativeUnitData unit)
        {
            foreach (var item in _pool.UsingItems)
            {
                item.InitiativeMarker.gameObject
                    .SetActive(item.Target.Unit == unit.Unit);
            }
        }

        private void OnUnitAdded(InitiativeQueueChangedEvent ev)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(AddUnitAsync(ev, ct));
        }

        private void OnUnitRemoved(InitiativeQueueChangedEvent ev)
        {
            var itemView = _pool.UsingItems.FirstOrDefault(x => x.Target.Unit == ev.ChangedUnit.Unit);
            if (itemView == null)
                return;
            
            itemView.transform.DOLocalMoveX(_itemAnimationDelta, _itemAnimationDuration)
                .OnComplete(() =>
                {
                    itemView.Target = default;
                    _pool.Return(itemView);
                });
        }

        private async UniTask AddUnitAsync(
            InitiativeQueueChangedEvent ev,
            CancellationToken ct)
        {
            var itemView = await _pool.GetAsync(ct);
            if (ct.IsCancellationRequested) return;
            
            itemView.Target = ev.ChangedUnit;
            itemView.InitiativeMarker.gameObject.SetActive(false);

            InsertObjectInInitiativeOrder(itemView, ev.ChangedUnit.Initiative);

            var pos = itemView.transform.localPosition;
            pos.x = _itemAnimationDelta;
            itemView.transform.localPosition = pos;

            itemView.InitiativeRollText.text = ev.ChangedUnit.Initiative.ToString();
            itemView.InitiativeRollPanel.gameObject.SetActive(true);
            
            itemView.gameObject.SetActive(true);
            itemView.transform.DOLocalMoveX(0f, _itemAnimationDuration);

            await UniTask.WaitForSeconds(_showInitiativeDuration, cancellationToken: ct);

            var oldPos = itemView.InitiativeRollPanel.transform.localPosition;
            itemView.InitiativeRollPanel.transform.DOLocalMoveX(0f, _itemAnimationDuration)
                .OnComplete(() =>
                {
                    itemView.InitiativeRollPanel.transform.localPosition = oldPos;
                    itemView.InitiativeRollPanel.gameObject.SetActive(false);
                });
        }

        private void InsertObjectInInitiativeOrder(InitiativeQueueViewItem item, int initiative)
        {
            _pool.UsingItems.Remove(item);
            for (int i = 0; i < _pool.UsingItems.Count; i++)
            {
                if (_pool.UsingItems[i].Target.Initiative < initiative)
                {
                    _pool.UsingItems.Insert(i, item);
                    item.transform.SetSiblingIndex(i);
                    return;
                }
            }
            
            _pool.UsingItems.Add(item);
            item.transform.SetSiblingIndex(_pool.UsingItems.Count - 1);
        }

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.InitializeAsync(ct, CreateItemsAtStartCount));
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }
    }
}