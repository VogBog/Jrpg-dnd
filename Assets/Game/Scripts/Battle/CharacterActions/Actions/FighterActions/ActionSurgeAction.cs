using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Characters.CharacterResources.DefaultResources;
using Game.Scripts.DataStorage.FreqDataStorage;
using Game.Scripts.Events.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Actions.FighterActions
{
    public class ActionSurgeAction : BaseCharacterAction
    {
        [Inject] private IDataStorage _storage;
        [Inject] private IEventBus _bus;
        
        [SerializeField] private AssetReferenceT<CharacterResourceData> _resourceReference;
        
        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            return CanUse();
        }

        protected override async UniTask OnUse(CancellationToken ct)
        {
            var resource = await _storage.LoadT(_resourceReference);
            ResourcesHolder.Add(resource, 1);
            _bus.Subscribe<TurnEndingEvent>(OnTurnEnded).When(x => x.UnitData.Unit == Owner);
        }

        protected override void ClearDataAfterUse()
        {
            
        }

        private async UniTask OnTurnEnded(TurnEndingEvent ev, CancellationToken ct)
        {
            _bus.Unsubscribe<TurnEndingEvent>(OnTurnEnded);
            var resource = await _storage.LoadT(_resourceReference);
            ResourcesHolder.RemoveSome(resource, 1);
        }

        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { Owner };
    }
}