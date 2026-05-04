using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Scenes;
using Game.Scripts.UI.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.CharacterAnalyze
{
    [RequireComponent(typeof(CharacterAnalyzeModelView))]
    public class CharacterAnalyzeStatusEffectsView : MonoBehaviour
    {
        [SerializeField] private UIObjectsPool<CharacterAnalyzeStatusEffectsViewItem> _pool;
        [SerializeField] private GameObject _infoPanel;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;

        private IAsyncOperationScope _scope;
        private CharacterAnalyzeModelView _mw;
        private IStatusEffect _showedStatusEffect;

        [Inject]
        private void Construct(IAsyncOperationScope scope)
        {
            _scope = scope;
            _mw = GetComponent<CharacterAnalyzeModelView>();
        }

        private void Awake()
        {
            _pool.InstantiatedOne += OnViewItemInitialized;
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.InitializeAsync(ct));
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        private void OnEnable()
        {
            _mw.StatusEffectsChanged += OnStatusEffectsChanged;
            _mw.UnitChanged += OnUnitChanged;
        }

        private void OnDisable()
        {
            _mw.StatusEffectsChanged -= OnStatusEffectsChanged;
            _mw.UnitChanged -= OnUnitChanged;
        }

        private void OnViewItemInitialized(CharacterAnalyzeStatusEffectsViewItem item)
        {
            item.PointerEntered += ShowEffectInfo;
            item.PointerExited += HideEffectInfo;
        }

        private void OnUnitChanged(CharacterAnalyzeRedrawData redrawData)
        {
            OnStatusEffectsChanged(redrawData.StatusEffects);
        }

        private void OnStatusEffectsChanged(IEnumerable<IStatusEffect> statusEffects)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(UpdateStatusEffectsAsync(statusEffects, ct));
        }

        private async UniTask UpdateStatusEffectsAsync(IEnumerable<IStatusEffect> statusEffects, CancellationToken ct)
        {
            _pool.ReturnAll();
            if (statusEffects == null)
                return;
            
            foreach (var statusEffect in statusEffects)
            {
                var item = await _pool.GetAsync(ct);
                item.Icon.sprite = statusEffect.Icon;
                item.Target = statusEffect;
                item.gameObject.SetActive(true);
            }
        }

        private void ShowEffectInfo(IStatusEffect effect)
        {
            _showedStatusEffect = effect;
            _icon.sprite = effect.Icon;
            _name.text = effect.Name;
            _description.text = effect.Description;
            _infoPanel.SetActive(true);
        }

        private void HideEffectInfo(IStatusEffect effect)
        {
            if (_showedStatusEffect != effect)
                return;

            _showedStatusEffect = null;
            _infoPanel.SetActive(false);
        }
    }
}