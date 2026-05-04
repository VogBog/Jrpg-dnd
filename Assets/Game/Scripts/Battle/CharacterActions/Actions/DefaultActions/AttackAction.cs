using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.BattleAnimations.Interfaces;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Battle.DamageDealing.Health.Interfaces;
using Game.Scripts.Battle.DamageDealing.Weapon;
using Game.Scripts.Battle.TargetChooser.Data;
using Game.Scripts.Battle.TargetChooser.Interfaces;
using Game.Scripts.Characters.ModularCharacters;
using Game.Scripts.Characters.Stats.Implementations;
using Game.Scripts.Characters.Stats.Interfaces;
using Game.Scripts.Rolls.Interfaces;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.DefaultActions
{
    public class AttackAction : BaseCharacterAction, IActiveAction
    {
        private WeaponHolder _weaponHolder;
        private ICheckRoller _checkRoller;
        private ITargetChooser _targetChooser;
        private IBattleAnimationsPlayer _animations;
        private IBattleUnitAnimator _animator;

        private IBattleUnit _chosenTarget;
        
        [Inject]
        private void Construct(
            IBattleUnit unit,
            ICheckRoller checkRoller,
            ITargetChooser targetChooser,
            IBattleAnimationsPlayer animations,
            IBattleUnitAnimator animator)
        {
            _weaponHolder = unit.GameObject.GetComponent<WeaponHolder>();
            _checkRoller = checkRoller;
            _targetChooser = targetChooser;
            _animations = animations;
            _animator = animator;
        }

        public override bool CanUse() => base.CanUse() && _weaponHolder != null && _weaponHolder.Item != null;

        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            if (!CanUse())
                return false;

            var command = new ChooseTargetCommand(new TargetFilter { Enemies = true }, 1);
            var targets = await _targetChooser.Choose(command, ct);

            if (targets.Count == 0)
                return false;

            _chosenTarget = targets[0];
            return true;
        }

        protected override async UniTask OnUse(CancellationToken ct)
        {
            await _animations.PlayCloseCombatAsync(
                Owner,
                _animator,
                _chosenTarget,
                OnAttack,
                ct);
        }

        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { _chosenTarget };

        private async UniTask OnAttack(CancellationToken ct)
        {
            var attackRoll = await _checkRoller.AttackRoll(
                _chosenTarget,
                _weaponHolder,
                ct);

            if (!attackRoll.Success || !_chosenTarget.GameObject.TryGetComponent(out IHealthProcessor health))
                return;
            
            ct.ThrowIfCancellationRequested();

            var damageList = new DamageList(new List<DamageValue>(_weaponHolder.Item.DamageList.Values.Values));
            if (Owner.GameObject.TryGetComponent(out IStatsTable stats) && 
                damageList.Values != null &&
                damageList.Values.Count > 0)
            {
                var value = damageList.Values[0];
                value.DamageBonus += stats.GetHighestModificator(_weaponHolder.Item.UsingStats);
                damageList.Values[0] = value;
            }

            var damageRoll = await _checkRoller.DamageRoll(
                _chosenTarget,
                Owner,
                damageList,
                ct);
            
            ct.ThrowIfCancellationRequested();

            var damageCommand = new DealDamageCommand(Owner, damageRoll, attackRoll.CriticalHit);
            await health.DealDamage(damageCommand, ct);
        }

        protected override void ClearDataAfterUse()
        {
            _chosenTarget = null;
        }
    }
}