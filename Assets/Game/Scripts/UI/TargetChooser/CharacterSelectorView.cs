using Game.Scripts.Battle;
using Game.Scripts.Battle.CharacterAnalyze.Interfaces;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.TargetChooser
{
    public class CharacterSelectorView : MonoBehaviour
    {
        [SerializeField] private GameObject _selectorItem;
        [SerializeField] private GameObject _healthObject;
        [SerializeField] private Image _healthBar;

        private ICharacterSelector _selector;
        private Camera _camera;

        [Inject]
        private void Construct(ICharacterSelector selector)
        {
            _selector = selector;
        }

        private void Awake()
        {
            OnSelectionChanged(null);
        }

        private void OnEnable()
        {
            _selector.Selected += OnSelectionChanged;
        }

        private void OnDisable()
        {
            _selector.Selected -= OnSelectionChanged;
        }

        private void OnSelectionChanged(IBattleUnit unit)
        {
            _selectorItem.gameObject.SetActive(unit != null);
            if (unit == null)
                return;
            
            _camera ??= Camera.main;
            if (_camera == null)
                return;
            
            var pos = _camera.WorldToScreenPoint(unit.GameObject.transform.position);
            pos.z = 0;
            _selectorItem.transform.position = pos;

            if (unit.GameObject.TryGetComponent(out IHealthProcessor health) &&
                health.MaxValue > 0)
            {
                _healthObject.SetActive(true);
                _healthBar.fillAmount = (float)health.Value / health.MaxValue;
            }
            else
            {
                _healthObject.SetActive(false);
            }
        }
    }
}