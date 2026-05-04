using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.BattleAnimations.Interfaces;
using Game.Scripts.Battle.CharacterActions.Events;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Characters.ModularCharacters;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Implementations
{
    public abstract class BaseSpell : BaseAction<BeforeUsingSpellEvent, UsingSpellEvent, UsedSpellEvent>
    {
        private ICharacterActionsHolder _actionsHolder;
        private IBattleAnimationsPlayer _animations;

        [Inject]
        private void ConstructBaseSpell(IBattleUnit unit, IBattleAnimationsPlayer animations)
        {
            _actionsHolder = unit.GameObject.GetComponent<ICharacterActionsHolder>();
            _animations = animations;
        }

        protected override UniTask OnUse(CancellationToken ct)
        {
            var targets = GetTargets();
            Transform target = Owner.GameObject.transform;
            if (targets != null && targets.Count > 0)
                target = targets[0].GameObject.transform;

            return _animations.PlayMagicUseAsync(
                Owner,
                Owner.UnitContainer.TryResolve<IBattleUnitAnimator>(),
                target,
                OnAnimatedUse,
                ct);
        }

        protected abstract UniTask OnAnimatedUse(CancellationToken ct);

        protected abstract List<IBattleUnit> GetTargets();
        
        protected override BeforeUsingSpellEvent SetBeforeUsingActionData(BeforeUsingSpellEvent beforeUseEvent)
        {
            return beforeUseEvent.SetData(Owner, this, ResourcesHolder, _actionsHolder);
        }

        protected override UsingSpellEvent SetUsingActionData(UsingSpellEvent usingEvent)
        {
            return usingEvent.SetData(Owner, this, ResourcesHolder, _actionsHolder);
        }

        protected override UsedSpellEvent SetUsedActionData(UsedSpellEvent usedEvent)
        {
            return usedEvent.SetData(Owner, ResourcesHolder, _actionsHolder, this, GetTargets());
        }

        protected override bool GetCancelAndClearBeforeUseEvent(BeforeUsingSpellEvent beforeUseEvent)
        {
            return beforeUseEvent.Cancel;
        }

        protected override bool GetCancelAndClearUsingEvent(UsingSpellEvent usingEvent)
        {
            bool cancelled = usingEvent.Cancel;
            usingEvent.Clear();
            return cancelled;
        }

        protected override void ClearUsedEvent(UsedSpellEvent usedEvent)
        {
            usedEvent.Clear();
        }
    }
}