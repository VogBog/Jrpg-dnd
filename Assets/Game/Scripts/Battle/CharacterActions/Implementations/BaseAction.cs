using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Characters.CharacterResources.Data;
using Game.Scripts.Characters.CharacterResources.DefaultResources;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using Game.Scripts.DataStorage.FreqDataStorage;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Implementations
{
    public abstract class BaseAction<TBeforeUseEvent, TUsingEvent, TUsedEvent> : MonoAction, ICharacterAction
        where TBeforeUseEvent : class
        where TUsingEvent : class
        where TUsedEvent : class
    {
        protected ICharacterResourcesHolder ResourcesHolder;
        protected IEventPool EventPool;
        protected IEventBus EventBus;
        protected ICharacterResourceMarkLevelChooser MarkLevelChooser;

        [SerializeField] private AssetReferenceT<CharacterResourceData>[] _useResources;
        [SerializeField] private MarkedResourceRequirementSerializable[] _useMarkedResources;
        
        public bool InUse { get; private set; } = false;

        [Inject]
        private void Construct(
            IBattleUnit battleUnit,
            IEventPool eventPool,
            IEventBus eventBus,
            ICharacterResourceMarkLevelChooser characterResourceMarkLevelChooser,
            IAsyncOperationScope scope,
            IDataStorage storage)
        {
            ResourcesHolder = battleUnit.GameObject.GetComponent<ICharacterResourcesHolder>();
            EventPool = eventPool;
            EventBus = eventBus;
            MarkLevelChooser = characterResourceMarkLevelChooser;
            
            if (scope.TryGetToken(out var ct))
                scope.FireAndForget(LoadResources(storage, ct));
        }
        
        public override bool CanUse()
        {
            foreach (var resourceType in UseResources)
            {
                if (!ResourcesHolder.Get(resourceType).CanUse())
                    return false;
            }

            foreach (var resourceType in UseMarkedResources)
            {
                if (ResourcesHolder
                        .GetMaxMarkLevelOf(resourceType.Data, true)
                    < resourceType.MarkValue)
                    return false;
            }

            return true;
        }
        
        public async UniTask<bool> Use(CancellationToken ct)
        {
            InUse = true;

            if (InvokeBeforeUsingAction())
            {
                InUse = false;
                return false;
            }

            var markLevels = new List<(MarkedResourceRequirementSerializable, int)>();
            if (!await ChooseMarkLevels(markLevels, ct))
                return false;

            bool readyForUse = await BeforeUse(ct);
            if (!readyForUse)
            {
                InUse = false;
                ClearDataAfterUse();
                return false;
            }

            if (await InvokeUsingAction(ct))
            {
                InUse = false;
                ClearDataAfterUse();
                return false;
            }
            
            foreach (var resourceType in UseResources)
            {
                ResourcesHolder.Get(resourceType).Use();
            }

            foreach (var (markedResource, level) in markLevels)
            {
                ResourcesHolder
                    .GetMarked(markedResource.Data, level)
                    .Use();
            }

            await OnUse(ct);

            await InvokeActionUsed(ct);
            InUse = false;
            ClearDataAfterUse();
            return true;
        }
        
        protected abstract UniTask<bool> BeforeUse(CancellationToken ct);
        protected abstract UniTask OnUse(CancellationToken ct);
        protected abstract void ClearDataAfterUse();

        private async UniTask LoadResources(IDataStorage storage, CancellationToken ct)
        {
            UseMarkedResources = _useMarkedResources;
            
            await UniTask.NextFrame();
            var tasks = new List<UniTask<CharacterResourceData>>();
            var tasks2 = new List<UniTask>();
            foreach (var resource in _useResources)
            {
                tasks.Add(storage.LoadT(resource));
            }

            foreach (var resource in _useMarkedResources)
            {
                tasks2.Add(resource.Map(storage));
            }

            var results = await UniTask.WhenAll(tasks);
            await UniTask.WhenAll(tasks2);
            
            UseResources = results;
        }

        private async UniTask<bool> ChooseMarkLevels(
            List<(MarkedResourceRequirementSerializable, int)> list,
            CancellationToken ct)
        {
            foreach (var markedResource in UseMarkedResources)
            {
                int maxUseLevel = ResourcesHolder.GetMaxMarkLevelOf(markedResource.Data, true);
                if (maxUseLevel < markedResource.MarkValue)
                    return false;
                if (maxUseLevel == markedResource.MarkValue)
                    continue;
                
                var command = new ChooseMarkLevelCommand(ResourcesHolder, markedResource.Data, markedResource.MarkValue);
                var (success, level) = await MarkLevelChooser.ChooseMarkLevel(command, ct);
                if (!success)
                    return false;
                
                list.Add((markedResource, level));
                ct.ThrowIfCancellationRequested();
            }

            return true;
        }
        
        protected abstract TBeforeUseEvent SetBeforeUsingActionData(TBeforeUseEvent beforeUseEvent);
        protected abstract TUsingEvent SetUsingActionData(TUsingEvent usingEvent);
        protected abstract TUsedEvent SetUsedActionData(TUsedEvent usedEvent);
        
        protected abstract bool GetCancelAndClearBeforeUseEvent(TBeforeUseEvent beforeUseEvent);
        protected abstract bool GetCancelAndClearUsingEvent(TUsingEvent usingEvent);
        protected abstract void ClearUsedEvent(TUsedEvent usedEvent);

        private bool InvokeBeforeUsingAction()
        {
            var beforeUsingEvent = SetBeforeUsingActionData(EventPool.Get<TBeforeUseEvent>());
            EventBus.Publish(beforeUsingEvent);

            bool cancelled = GetCancelAndClearBeforeUseEvent(beforeUsingEvent);
            EventPool.Return(beforeUsingEvent);

            return cancelled;
        }

        private async UniTask<bool> InvokeUsingAction(CancellationToken ct)
        {
            var usingAction = SetUsingActionData(EventPool.Get<TUsingEvent>());
            await EventBus.Publish(usingAction, ct);

            bool cancelled = GetCancelAndClearUsingEvent(usingAction);
            EventPool.Return(usingAction);

            return cancelled;
        }

        private async UniTask InvokeActionUsed(CancellationToken ct)
        {
            var ev = SetUsedActionData(EventPool.Get<TUsedEvent>());
            
            await EventBus.Publish(ev, ct);
            
            ClearUsedEvent(ev);
            EventPool.Return(ev);
        }
    }
}