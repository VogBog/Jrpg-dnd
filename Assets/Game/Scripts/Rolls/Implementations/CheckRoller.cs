using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle;
using Game.Scripts.Battle.DamageDealing.Armor;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Battle.DamageDealing.Weapon;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Characters.Stats.Data;
using Game.Scripts.Characters.Stats.Interfaces;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Rolls.Data;
using Game.Scripts.Rolls.Events;
using Game.Scripts.Rolls.Interfaces;
using Zenject;

namespace Game.Scripts.Rolls.Implementations
{
    public class CheckRoller : ICheckRoller
    {
        [Inject] private IEventPool _pool;
        [Inject] private IEventBus _bus;
        [Inject] private IDiceRoller _diceRoller;
        
        public async UniTask<RollResult> InitiativeCheck(
            IBattleUnit unit,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder = null)
        {
            //Parameters
            var parameters = _pool.Get<RollDiceParameters>().Init(10);
            if (unit.GameObject.TryGetComponent(out IStatsTable statsTable))
                parameters.AddBonus(statsTable.GetModifier(Stats.DEX));
            
            parametersBuilder?.Invoke(parameters);

            //Before
            var rollingEvent = _pool.Get<InitiativeRollingEvent>().Init(unit, statsTable, parameters);
            await _bus.Publish(rollingEvent, ct);
            _pool.Return(rollingEvent);
            
            //Roll
            var result = await _diceRoller.Roll(parameters, ct);

            //After
            var rolledEvent = _pool.Get<InitiativeRolledEvent>().Init(unit, result);
            await _bus.Publish(rolledEvent, ct);
            _pool.Return(rolledEvent);
            _pool.Return(parameters);

            //Set result
            return result;
        }

        public async UniTask<bool> SaveThrow(
            IBattleUnit target,
            IBattleUnit attacker,
            Stats stat,
            int difficulty,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder = null)
        {
            //Parameters
            var parameters = _pool.Get<RollDiceParameters>().Init(20);
            if (target.GameObject.TryGetComponent(out IStatsTable statsTable))
            {
                parameters.AddBonus(statsTable.GetModifier(stat));
            }
            parametersBuilder?.Invoke(parameters);
            
            //Before
            var rollingEvent = _pool.Get<SaveThrowRollingEvent>()
                .Init(target, attacker, statsTable, parameters, stat, difficulty);
            await _bus.Publish(rollingEvent, ct);
            
            //Roll
            var rollResult = await _diceRoller.Roll(parameters, ct);
            
            //After
            var rolledEvent = _pool.Get<SaveThrowRolledEvent>()
                .Init(rollingEvent, rollResult.Result, rollResult.Result >= difficulty);
            
            await _bus.Publish(rolledEvent, ct);

            bool success = rolledEvent.Success;
            
            _pool.Return(rolledEvent);
            _pool.Return(rollingEvent);
            _pool.Return(parameters);

            //Set result
            return success;
        }

        public async UniTask<AttackRollResults> AttackRoll(
            IBattleUnit target,
            WeaponHolder attacker,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder = null)
        {
            //Parameters
            var parameters = _pool.Get<RollDiceParameters>().Init(20);
            if (attacker.Owner.GameObject.TryGetComponent(out IStatsTable statsTable))
            {
                var stats = attacker.Item.UsingStats;
                if (stats.Length > 0)
                {
                    int maxStat = stats.Max(x => statsTable.GetModifier(x));
                    parameters.AddBonus(maxStat);
                }

                parameters.AddBonus(statsTable.MasteryBonus);
            }
            parametersBuilder?.Invoke(parameters);
            
            //Before
            var rollingEvent = _pool.Get<AttackRollingEvent>().Init(target, attacker.Owner, parameters, attacker);
            await _bus.Publish(rollingEvent, ct);
            if (rollingEvent.Cancelled)
            {
                _pool.Return(rollingEvent);
                return new AttackRollResults(0, false, false, true);
            }
            
            //Roll
            return await AttackRollAfterPrepares(rollingEvent, ct);
        }

        public async UniTask<AttackRollResults> AttackRollNotWeapon(IBattleUnit target, IBattleUnit attacker, CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder = null)
        {
            //Parameters
            var parameters = _pool.Get<RollDiceParameters>().Init(20);
            parametersBuilder?.Invoke(parameters);
            
            //Before
            var rollingEvent = _pool.Get<AttackRollingEvent>().Init(target, attacker, parameters, null);
            await _bus.Publish(rollingEvent, ct);

            return await AttackRollAfterPrepares(rollingEvent, ct);
        }

        public async UniTask<RolledDamageList> DamageRoll(
            IBattleUnit target,
            IBattleUnit attacker,
            DamageList damageList,
            CancellationToken ct)
        {
            if (damageList.Values == null)
                return default;

            var rollingEvent = _pool.Get<DamageRollingEvent>().Init(attacker, target, damageList);
            await _bus.Publish(rollingEvent, ct);

            var resultList = new List<RolledDamageValue>();
            foreach (var value in damageList.Values)
            {
                //Parameters
                var parameters = _pool.Get<RollDiceParameters>()
                    .Init(value.Dices.Count, value.Dices.Dice)
                    .AddBonus(value.DamageBonus);
                
                //Before
                var rollingDamageTypeEvent =
                    _pool.Get<DamageSingleTypeRollingEvent>().Init(attacker, target, parameters);
                await _bus.Publish(rollingDamageTypeEvent, ct);
                
                //Roll
                var rollResult = await _diceRoller.Roll(parameters, ct);
                var damageResult = new RolledDamageValue(value.DamageType, rollResult.Result);
                resultList.Add(damageResult);
                
                _pool.Return(rollingDamageTypeEvent);
                _pool.Return(parameters);
            }
            
            //After
            var rolledDamageList = new RolledDamageList(resultList);
            var rolledEvent = _pool.Get<DamageRolledEvent>().Init(attacker, target, rolledDamageList);
            await _bus.Publish(rolledEvent, ct);

            rolledDamageList = rolledEvent.DamageList;
            _pool.Return(rolledDamageList);
            _pool.Return(rollingEvent);
            
            return rolledDamageList;
        }

        public async UniTask<int> HealRoll(
            IBattleUnit target,
            IBattleUnit caster,
            CancellationToken ct,
            Action<RollDiceParameters> parametersBuilder)
        {
            //Parameters
            var parameters = _pool.Get<RollDiceParameters>();
            parametersBuilder.Invoke(parameters);
            
            //Before
            var rollingEvent = _pool.Get<HealRollingEvent>().Init(target, caster, parameters);
            await _bus.Publish(rollingEvent, ct);
            _pool.Return(rollingEvent);
            
            //Roll
            var rollResult = await _diceRoller.Roll(parameters, ct);

            return rollResult.Result;
        }

        private async UniTask<AttackRollResults> AttackRollAfterPrepares(
            AttackRollingEvent rollingEvent,
            CancellationToken ct)
        {
            //Roll
            var rollResult = await _diceRoller.Roll(rollingEvent.Parameters, ct);
            
            //After
            var rolledEvent = _pool.Get<AttackRolledEvent>().Init(rollingEvent, rollResult);
            await _bus.Publish(rolledEvent, ct);

            var attackResult = new AttackRollResults(
                rolledEvent.Result, false, rolledEvent.CriticalHit, rolledEvent.CriticalMiss);
            
            //Set result
            if (rollingEvent.Target.GameObject.TryGetComponent(out Armor armor))
                attackResult.Success = attackResult.Roll >= armor.Value;
            if (rolledEvent.CriticalHit)
                attackResult.Success = true;
            if (rolledEvent.CriticalMiss)
                attackResult.Success = false;
            
            //Last publish
            var lastEv = _pool.Get<AttackPerformingEvent>().Init(rolledEvent, attackResult);
            await _bus.Publish(lastEv, ct);
            
            _pool.Return(lastEv);
            _pool.Return(rolledEvent);
            _pool.Return(rollingEvent);
            _pool.Return(rollingEvent.Parameters);

            return attackResult;
        }
    }
}