using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle;
using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Scenes;
using Game.Scripts.UI.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.CharacterAnalyze
{
    [RequireComponent(typeof(CharacterAnalyzeModelView))]
    public class CharacterAnalyzeView : MonoBehaviour
    {
        private IAsyncOperationScope _scope;
        private CharacterAnalyzeModelView _mw;

        public const int PreInstantiateItemsCount = 8;

        [SerializeField] private GameObject _window;
        [SerializeField] private GameObject _openButton;
        [SerializeField] private UIObjectsPool<CharacterAnalyzeQueueItem> _pool;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private Image _healthSlider;
        [SerializeField] private TMP_Text _armorText;

        [SerializeField] private AssetReferenceObject<CharacterAnalyzeStatView> _statViewPrefabReference;
        [SerializeField] private Transform _statViewTable;

        private CharacterAnalyzeStatView[] _statViews;

        public event Action<CharacterAnalyzeQueueItem> QueueItemCreated;
        public event Action<CharacterAnalyzeStatView> StatViewCreated; 

        [Inject]
        private void Construct(IAsyncOperationScope scope)
        {
            _scope = scope;
            _mw = gameObject.GetComponent<CharacterAnalyzeModelView>();
        }

        private void OnEnable()
        {
            _mw.Opened += OnOpened;
            _mw.Closed += OnClosed;
            _mw.UnitChanged += OnUnitChanged;
            _mw.HealthChanged += OnHealthChanged;
            _mw.ArmorChanged += OnArmorChanged;
            _pool.InstantiatedOne += OnQueueItemInstantiated;
        }

        private void OnDisable()
        {
            _mw.Opened -= OnOpened;
            _mw.Closed -= OnClosed;
            _mw.UnitChanged -= OnUnitChanged;
            _mw.HealthChanged -= OnHealthChanged;
            _mw.ArmorChanged -= OnArmorChanged;
            _pool.InstantiatedOne -= OnQueueItemInstantiated;
        }

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(InitializeAsync(ct));
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            var prefab = await _statViewPrefabReference.LoadAssetAsync();
            int enumCount = Enum.GetValues(typeof(Stats)).Length;
            _statViews = new CharacterAnalyzeStatView[enumCount];
            
            for (int i = 0; i < enumCount; i++)
            {
                var instance = Instantiate(prefab, _statViewTable);
                _statViews[i] = instance;
                StatViewCreated?.Invoke(instance);
            }
            
            _statViewPrefabReference.ReleaseIfValid();
            if (ct.IsCancellationRequested)
                return;
            
            await _pool.InitializeAsync(ct, PreInstantiateItemsCount);
        }

        private void OnOpened(IEnumerable<IBattleUnit> queue)
        {
            _openButton.gameObject.SetActive(false);
            _pool.ReturnAll();
            _window.SetActive(true);
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(OpenAsync(queue, ct));
        }

        private async UniTask OpenAsync(IEnumerable<IBattleUnit> queue, CancellationToken ct)
        {
            foreach (var unit in queue)
            {
                var item = await _pool.GetAsync(ct);
                item.gameObject.SetActive(true);
                item.Target = unit;
                item.Image.sprite = unit.Icon;
            }
        }

        private void OnClosed()
        {
            _openButton.gameObject.SetActive(true);
            _window.SetActive(false);
            _pool.ReturnAll();
        }

        private void OnQueueItemInstantiated(CharacterAnalyzeQueueItem item)
        {
            QueueItemCreated?.Invoke(item);
        }

        private void OnUnitChanged(CharacterAnalyzeRedrawData data)
        {
            _nameText.text = data.Name;
            _healthText.text =  "Invincible";
            _armorText.text = data.InfiniteArmor ? "Invincible" : data.Armor.ToString();
            _avatarImage.sprite = data.Avatar;
            
            if (!data.InfiniteHealth)
                OnHealthChanged(data.Health, data.MaxHealth);

            foreach (var statView in _statViews) 
                statView.gameObject.SetActive(data.Stats != null);

            if (data.Stats == null)
                return;

            if (data.Stats.Length != _statViews.Length)
                throw new Exception(
                    $"CharacterAnalyzeView: statViews count must be equals to stats count - {data.Stats.Length}");
            
            for (int i = 0; i < data.Stats.Length; i++)
            {
                var statView = _statViews[i];
                var statInfo = data.Stats[i];

                statView.NameText.text = statInfo.Stat.ToString();
                statView.ValueText.text = statInfo.Value.ToString();
                statView.ModifierText.text = GetModifierPrettyString(statInfo.Modifier);
                statView.SaveThrowText.text = GetModifierPrettyString(statInfo.SaveThrow);
                statView.HaveSaveThrow.gameObject.SetActive(statInfo.HaveSaveThrow);
            }
        }

        private void OnHealthChanged(int health, int maxHealth)
        {
            if (maxHealth == 0)
                throw new DivideByZeroException($"CharacterAnalyzeView.OnHealthChanged: maxHealth must be not equal to 0");
            
            _healthText.text = $"{health}/{maxHealth}";
            _healthSlider.fillAmount = (float)health / maxHealth;
        }

        private void OnArmorChanged(int armor)
        {
            _armorText.text = armor.ToString();
        }

        private string GetModifierPrettyString(int modifier)
        {
            if (modifier >= 0)
                return $"+{modifier}";
            return modifier.ToString();
        }
    }
}