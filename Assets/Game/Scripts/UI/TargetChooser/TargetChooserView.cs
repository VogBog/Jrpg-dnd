using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle;
using Game.Scripts.Battle.TargetChooser.Data;
using Game.Scripts.Battle.TargetChooser.Interfaces;
using Game.Scripts.Scenes;
using Game.Scripts.UI.Helpers;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.TargetChooser
{
    public class TargetChooserView : MonoBehaviour
    {
        private IPlayerTargetChooser _targetChooser;
        private IAsyncOperationScope _scope;

        [SerializeField] private TMP_Text _chooseTargetText;
        [SerializeField, TextArea] private string _chooseTargetTemplate;
        [SerializeField] private UIObjectsPool<TargetChooserViewItem> _pool;

        [Inject]
        private void Construct(IPlayerTargetChooser targetChooser, IAsyncOperationScope scope)
        {
            _targetChooser = targetChooser;
            _scope = scope;
        }

        public void Submit()
        {
            _targetChooser.Submit();
        }

        public void Close()
        {
            _targetChooser.Cancel();
        }

        private void OnEnable()
        {
            _targetChooser.SelectionStarted += OnStartSelecting;
            _targetChooser.SelectionEnded += OnStopSelecting;
            _targetChooser.SelectedUnitsChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            _targetChooser.SelectionStarted -= OnStartSelecting;
            _targetChooser.SelectionEnded -= OnStopSelecting;
            _targetChooser.SelectedUnitsChanged -= OnSelectionChanged;
        }

        private void Awake()
        {
            _chooseTargetText.gameObject.SetActive(false);
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.InitializeAsync(ct));
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        private void OnStartSelecting(ChooseTargetCommand command)
        {
            _chooseTargetText.text = string.Format(_chooseTargetTemplate, command.MaxCount);
            _chooseTargetText.gameObject.SetActive(true);
        }

        private void OnStopSelecting()
        {
            _chooseTargetText.gameObject.SetActive(false);
            _pool.ReturnAll();
        }

        private void OnSelectionChanged(IList<IBattleUnit> battleUnits, IBattleUnit changedUnit)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(RepaintAllAsync(battleUnits, changedUnit, ct));
        }

        private async UniTask RepaintAllAsync(IList<IBattleUnit> battleUnits, IBattleUnit changedUnit, CancellationToken ct)
        {
            var camera = Camera.main;
            for (int i = 0; i < battleUnits.Count; i++)
            {
                var usingItem = _pool.UsingItems.Count > i ? _pool.UsingItems[i] : await _pool.GetAsync(ct);
                PaintOne(battleUnits[i], usingItem, camera);
                    
                if (battleUnits[i] == changedUnit)
                {
                    usingItem.AnimateOpening();
                }
            }
            
            if (_pool.UsingItems.Count > battleUnits.Count)
                _pool.Return(_pool.UsingItems[^1]);
        }

        private void PaintOne(IBattleUnit unit, TargetChooserViewItem viewItem, Camera camera)
        {
            var screenPos = camera.WorldToScreenPoint(unit.GameObject.transform.position);
            screenPos.z = 0;
            viewItem.transform.position = screenPos;
        }
    }
}