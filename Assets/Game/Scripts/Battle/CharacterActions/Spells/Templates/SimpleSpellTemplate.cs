using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;
using Game.Scripts.Battle.StatusEffects.Data;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.Battle.TargetChooser.Data;
using Game.Scripts.Battle.TargetChooser.Interfaces;
using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Characters.Stats.Implementations;
using Game.Scripts.Characters.Stats.Interfaces;
using Game.Scripts.DataStorage.FreqDataStorage;
using Game.Scripts.Rolls.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Spells.Templates
{
    public class SimpleSpellTemplate : BaseSpell, IActiveAction
    {
        [Inject] private IBattleUnit _unit;
        [Inject] private ICheckRoller _checkRoller;
        [Inject] private ITargetChooser _targetChooser;
        [Inject] private IDataStorage _storage;
        [Inject] private BaseSpellAnimationsPlayer _baseSpellAnimations;
        
        [SerializeField] private int _targetsCount;
        [SerializeField] private TargetFilter _teamMask;
        
        [SerializeField] private bool _isAttackRoll;
        [SerializeField] private TemplateActions _onAttacked;

        [SerializeField] private bool _saveThrow;
        [SerializeField] private Stats _saveThrowStat;
        [SerializeField] private TemplateActions _onSaveThrowFailed;

        [SerializeField] private TemplateActions _always;

        private List<IBattleUnit> _targets;
        
        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            var command = new ChooseTargetCommand(_teamMask, _targetsCount);
            _targets = await _targetChooser.Choose(command, ct);
            ct.ThrowIfCancellationRequested();

            if (_targets == null || _targets.Count == 0)
                return false;

            return true;
        }

        protected override async UniTask OnAnimatedUse(CancellationToken ct)
        {
            foreach (var target in _targets)
            {
                await ExecuteTemplateActions(target, _always, false, false, ct);
                
                ct.ThrowIfCancellationRequested();
                
                if (_isAttackRoll)
                {
                    var damageList = await _onAttacked.Damage.Map(_storage, ct);
                    var mainDamageType = damageList.GetMainDamageType();

                    await _baseSpellAnimations.AnimateProjectileAsync(
                        Owner.GameObject.transform.position,
                        target.GameObject.transform.position,
                        mainDamageType,
                        ct,
                        async () =>
                        {
                            var attackRoll = await _checkRoller.AttackRollNotWeapon(
                                target,
                                Owner,
                                ct,
                                parameters =>
                                {
                                    if (Owner.GameObject.TryGetComponent(out IStatsTable statsTable))
                                        statsTable.SetSpellAttackRollBonuses(parameters);
                                });

                            if (!attackRoll.Success)
                                return;
                    
                            ct.ThrowIfCancellationRequested();

                            await ExecuteTemplateActions(target, _onAttacked, false, attackRoll.CriticalHit, ct);
                        });
                }
                
                ct.ThrowIfCancellationRequested();

                if (_saveThrow)
                {
                    var damageList = await _onSaveThrowFailed.Damage.Map(_storage, ct);

                    await _baseSpellAnimations.AnimateSaveThrow(
                        target.GameObject.transform.position,
                        damageList.GetMainDamageType(),
                        ct,
                        async () =>
                        {
                            int difficulty = 10;
                            if (Owner.GameObject.TryGetComponent(out IStatsTable statsTable))
                            {
                                difficulty = statsTable.GetSpellSaveThrowDifficulty();
                            }

                            var saveThrowRoll = await _checkRoller.SaveThrow(
                                target,
                                Owner,
                                _saveThrowStat,
                                difficulty,
                                ct);
                    
                            ct.ThrowIfCancellationRequested();

                            await ExecuteTemplateActions(target, _onSaveThrowFailed, saveThrowRoll, false, ct);
                        });
                }
                
                ct.ThrowIfCancellationRequested();
            }
        }

        protected override void ClearDataAfterUse()
        {
            _targets = null;
        }

        protected override List<IBattleUnit> GetTargets() => _targets;

        private async UniTask ExecuteTemplateActions(
            IBattleUnit target,
            TemplateActions actions,
            bool successSaveThrow,
            bool criticalHit,
            CancellationToken ct)
        {
            if (actions.DealDamage && (!successSaveThrow || actions.HalfIfSuccessSaveThrow) &&
                target.GameObject.TryGetComponent(out IHealthProcessor healthProcessor))
            {
                var damageList = await actions.Damage.Map(_storage, ct);
                
                var rolledDamage = await _checkRoller.DamageRoll(
                    target,
                    Owner,
                    damageList,
                    ct);

                if (successSaveThrow)
                {
                    for (int i = 0; i < rolledDamage.Values.Count; i++)
                    {
                        var value = rolledDamage.Values[i];
                        value.Value /= 2;
                        rolledDamage.Values[i] = value;
                    }
                }

                var command = new DealDamageCommand(Owner, rolledDamage, criticalHit);
                await healthProcessor.DealDamage(command, ct);
            }
            
            ct.ThrowIfCancellationRequested();

            if (!successSaveThrow &&
                actions.SetStatusEffect &&
                target.GameObject.TryGetComponent(out IStatusEffectProcessor statusEffectProcessor))
            {
                var effectPrefab = await _storage.LoadObject(actions.StatusEffect) as Object;
                var command = new AttachStatusEffectCommand(
                    Owner, effectPrefab, actions.StatusEffectEndTime, actions.StatusEffectDuration);
                statusEffectProcessor.Attach(command);
            }
            
            ct.ThrowIfCancellationRequested();

            if (actions.Heal && target.GameObject.TryGetComponent(out IHealthProcessor processor))
            {
                int heal = await _checkRoller.HealRoll(
                    target,
                    Owner,
                    ct,
                    parameters => parameters.Init(actions.HealParameters.Count, actions.HealParameters.Dice));

                var healCommand = new HealCommand(Owner, heal);
                await processor.Heal(healCommand, ct);
            }
        }
    }
}