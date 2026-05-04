using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterAnalyze.Interfaces;
using Game.Scripts.Battle.TargetChooser.Data;
using Game.Scripts.Battle.TargetChooser.Interfaces;
using Game.Scripts.Battle.UnitsTeam;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.TargetChooser.Implementations
{
    public class PlayerMouseTargetChooser : MonoBehaviour, IPlayerTargetChooser
    {
        [Inject] private ICharacterSelector _selector;
        
        private readonly List<IBattleUnit> _selectedUnits = new();
        private bool _selecting = false;
        private ChooseTargetCommand _command;
        
        public event Action<ChooseTargetCommand> SelectionStarted;
        public event Action<List<IBattleUnit>, IBattleUnit> SelectedUnitsChanged;
        public event Action SelectionEnded;
        
        public async UniTask<List<IBattleUnit>> Choose(ChooseTargetCommand command, CancellationToken ct)
        {
            if (_selecting)
                return null;
            
            SelectionStarted?.Invoke(command);
            _selecting = true;
            _command = command;
            _selectedUnits.Clear();
            
            await UniTask.WaitWhile(() => _selecting, cancellationToken: ct);

            _selecting = false;
            SelectionEnded?.Invoke();
            
            return new List<IBattleUnit>(_selectedUnits);
        }

        public void Submit()
        {
            _selecting = false;
        }

        public void Cancel()
        {
            _selectedUnits.Clear();
            _selecting = false;
        }

        private void Update()
        {
            if (!_selecting)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                var unit = _selector.SelectedUnit;
                if (unit == null)
                    return;
                
                if (_selectedUnits.Contains(unit))
                {
                    _selectedUnits.Remove(unit);
                }
                else if (_selectedUnits.Count < _command.MaxCount &&
                         _command.Filter.GetMaskForPlayer().HasTeam(UnitTeamsUtils.GetTeam(unit)))
                {
                    _selectedUnits.Add(unit);
                }
                    
                SelectedUnitsChanged?.Invoke(_selectedUnits, unit);

                if (_selectedUnits.Count == _command.MaxCount)
                    _selecting = false;
            }
        }
    }
}