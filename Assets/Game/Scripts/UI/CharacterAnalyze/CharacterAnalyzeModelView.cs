using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle;
using Game.Scripts.Battle.DamageDealing.Armor;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Battle.TargetChooser.Data;
using Game.Scripts.Battle.TargetChooser.Interfaces;
using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Characters.Stats.Interfaces;
using Game.Scripts.Scenes;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.CharacterAnalyze
{
    public class CharacterAnalyzeModelView : MonoBehaviour
    {
        private ITargetChooser _chooser;
        private IInitiativeQueue _queue;
        private IAsyncOperationScope _scope;
        
        [CanBeNull] private IBattleUnit _unit;
        [CanBeNull] private IStatsTable _statsTable;
        [CanBeNull] private IHealthProcessor _health;
        [CanBeNull] private Armor _armor;
        [CanBeNull] private IStatusEffectProcessor _statusEffects;

        public event Action<int, int> HealthChanged;
        public event Action<int> ArmorChanged;
        public event Action<IEnumerable<IStatusEffect>> StatusEffectsChanged; 
        public event Action<CharacterAnalyzeRedrawData> UnitChanged;

        public event Action<IEnumerable<IBattleUnit>> Opened;
        public event Action Closed;
        
        [Inject]
        private void Construct(ITargetChooser chooser, IInitiativeQueue queue, IAsyncOperationScope scope)
        {
            _chooser = chooser;
            _queue = queue;
            _scope = scope;
        }

        public void Close()
        {
            if (_unit != null)
            {
                ClearUnitData();
            }
            
            _unit = null;
            Closed?.Invoke();
        }

        public void Open()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(OpenAsync(ct));
        }

        public void Select([CanBeNull] IBattleUnit unit)
        {
            if (_unit != null)
            {
                ClearUnitData();
            }

            _unit = unit;

            if (_unit != null)
            {
                _statsTable = _unit.GameObject.GetComponent<IStatsTable>();
                _health = _unit.GameObject.GetComponent<IHealthProcessor>();
                _armor = _unit.GameObject.GetComponent<Armor>();
                _statusEffects = _unit.GameObject.GetComponent<IStatusEffectProcessor>();
                
                if (_health != null)
                    _health.Changed += OnHealthChanged;
                if (_armor != null)
                    _armor.Changed += OnArmorChanged;
                if (_statusEffects != null)
                    _statusEffects.Changed += OnStatusEffectChanged;

                var data = new CharacterAnalyzeRedrawData(
                    _unit.Name,
                    _health?.Value ?? 0,
                    _health?.MaxValue ?? 0,
                    _armor?.Value ?? 0,
                    GetStatsFrom(_statsTable),
                    _health?.GetDamageReactions(),
                    _statusEffects?.GetAll(true),
                    _health == null,
                    _armor == null);
                
                UnitChanged?.Invoke(data);
                return;
            }

            Close();
        }

        private void Awake()
        {
            Close();
        }

        private async UniTask OpenAsync(CancellationToken ct)
        {
            var command = new ChooseTargetCommand(TargetFilter.All(), 1);
            var targets = await _chooser.Choose(command, ct);

            if (targets.Count == 0 || ct.IsCancellationRequested)
                return;
            
            Opened?.Invoke(_queue.Queue.Select(x => x.Unit));
            Select(targets[0]);
        }

        private void ClearUnitData()
        {
            if (_health != null)
                _health.Changed -= OnHealthChanged;
            if (_armor != null)
                _armor.Changed -= OnArmorChanged;
            if (_statusEffects != null)
                _statusEffects.Changed -= OnStatusEffectChanged;
        }

        private void OnHealthChanged(IHealthProcessor health)
        {
            HealthChanged?.Invoke(health.Value, health.MaxValue);
        }

        private void OnArmorChanged(Armor armor)
        {
            ArmorChanged?.Invoke(armor.Value);
        }

        private void OnStatusEffectChanged(IStatusEffectProcessor statusEffectsProcessor)
        {
            StatusEffectsChanged?.Invoke(statusEffectsProcessor.GetAll(true));
        }

        private CharacterAnalyzeRedrawData.StatInfo[] GetStatsFrom(IStatsTable statsTable)
        {
            if (statsTable == null)
                return null;

            if (Enum.GetValues(typeof(Stats)) is not Stats[] statsEnum)
                return null;
            
            var result = new CharacterAnalyzeRedrawData.StatInfo[statsEnum.Length];
            for (int i = 0; i < statsEnum.Length; i++)
            {
                var stat = statsEnum[i];
                result[i] = new CharacterAnalyzeRedrawData.StatInfo(
                    stat,
                    statsTable.GetValue(stat),
                    statsTable.GetModifier(stat),
                    statsTable.GetSaveThrow(stat),
                    statsTable.HasSaveThrow(stat));
            }

            return result;
        }
    }
}