using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Characters.CharacterResources.Data;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using Game.Scripts.Scenes;
using Game.Scripts.UI.Helpers;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.MarkLevelChooser
{
    public class MarkLevelChooserView : MonoBehaviour
    {
        private IPlayerResourceMarkLevelChooser _chooser;
        private IAsyncOperationScope _scope;

        [SerializeField] private GameObject _panel;
        [SerializeField] private UIObjectsPool<MarkLevelChooserViewItem> _pool;

        [Inject]
        private void Construct(IPlayerResourceMarkLevelChooser chooser, IAsyncOperationScope scope)
        {
            _chooser = chooser;
            _scope = scope;
            _pool.InstantiatedOne += InstantiatedOneItem;
        }
        
        public void Open(ChooseMarkLevelCommand command)
        {
            _panel.SetActive(true);
            _pool.ReturnAll();

            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(OpenAsync(command, ct));
        }

        public void Close()
        {
            _pool.ReturnAll();
            _panel.SetActive(false);
        }

        public void Cancel()
        {
            _chooser.Cancel();
        }

        private void OnEnable()
        {
            _chooser.Opened += Open;
            _chooser.Closed += Close;
        }

        private void OnDisable()
        {
            _chooser.Opened -= Open;
            _chooser.Closed -= Close;
        }

        private void InstantiatedOneItem(MarkLevelChooserViewItem item)
        {
            item.Clicked += _chooser.Select;
        }

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.InitializeAsync(ct));
        }

        private async UniTask OpenAsync(ChooseMarkLevelCommand command, CancellationToken ct)
        {
            foreach (var resource in command.Holder.GetAllMarkedBy(command.Data))
            {
                var slot = await _pool.GetAsync(ct);

                var sprite = resource.Data.Icon;
                foreach (var variant in resource.MarkedData.Variants)
                {
                    if (resource.MarkValue == variant.MarkValue)
                    {
                        sprite = variant.Icon;
                        break;
                    }
                }

                var color = resource.Data.Color;
                if (!resource.CanUse())
                    color /= 2;

                slot.Icon.sprite = sprite;
                slot.Icon.color = color;
                slot.MarkLevel = resource.MarkValue;
                slot.Button.enabled = resource.CanUse();
                slot.gameObject.SetActive(true);
            }
        }
    }
}