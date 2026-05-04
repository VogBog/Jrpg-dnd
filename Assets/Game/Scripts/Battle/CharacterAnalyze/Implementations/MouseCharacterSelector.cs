using System;
using Game.Scripts.Battle.CharacterAnalyze.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Battle.CharacterAnalyze.Implementations
{
    public class MouseCharacterSelector : MonoBehaviour, ICharacterSelector
    {
        private Camera _camera;

        public IBattleUnit SelectedUnit { get; private set; }
        
        public event Action<IBattleUnit> Selected;
        
        public void Select(IBattleUnit unit)
        {
            SelectedUnit = unit;
            Selected?.Invoke(unit);
        }

        private void Update()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
                return;
            }

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                if (SelectedUnit != null)
                    Select(null);
                return;
            }
                
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f) &&
                hit.collider.TryGetComponent(out IBattleUnit unit))
            {
                Select(unit);
            }
            else
            {
                Select(null);
            }
        }
    }
}