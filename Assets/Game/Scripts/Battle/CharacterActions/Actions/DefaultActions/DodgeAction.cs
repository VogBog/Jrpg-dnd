using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.StatusEffects.Data;
using Game.Scripts.Battle.StatusEffects.Effects;
using Game.Scripts.Battle.StatusEffects.Interfaces;
using Game.Scripts.DataStorage.FreqDataStorage;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.DefaultActions
{
    public class DodgeAction : BaseCharacterAction
    {
        [Inject] private IDataStorage _dataStorage;
        
        [SerializeField] private AssetReferenceObject<DodgingStatusEffect> _dodgeEffectResource;

        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            return CanUse();
        }

        protected override async UniTask OnUse(CancellationToken ct)
        {
            if (!Owner.GameObject.TryGetComponent(out IStatusEffectProcessor statusEffectProcessor))
                return;
            
            var effect = await _dataStorage.LoadObject(_dodgeEffectResource);
            var command = new AttachStatusEffectCommand(
                Owner, effect, EndEffectTimes.TurnStart, 1);
            statusEffectProcessor.Attach(command);
        }

        protected override void ClearDataAfterUse()
        {
            
        }

        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { Owner };
    }
}