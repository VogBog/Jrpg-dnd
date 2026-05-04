using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Scenes;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Battle.Starter
{
    public class BattleUnitSummoner : IBattleUnitSummoner
    {
        private IAsyncOperationScope _scope;
        private IInitiativeQueue _initiativeQueue;
        private DiContainer _container;

        private readonly List<(IBattleUnit, AssetReferenceObject<IBattleUnit>)> _operations = new();
        
        [Inject]
        private void Construct(
            IAsyncOperationScope scope,
            IInitiativeQueue initiativeQueue,
            DiContainer container)
        {
            _scope = scope;
            _initiativeQueue = initiativeQueue;
            _container = container;
        }

        public async UniTask<IBattleUnit> SummonAsync(
            AssetReferenceObject<IBattleUnit> reference,
            bool lastInitiative,
            CancellationToken ct)
        {
            var prefab = await reference.LoadAssetAsync();
            if (prefab == null || ct.IsCancellationRequested)
            {
                reference.ReleaseIfValid();
                if (ct.IsCancellationRequested) throw new OperationCanceledException();
                throw new NullReferenceException($"BattleUnitSummoner.SummonAsync: {reference} gives nothing.");
            }

            var unit = _container.InstantiatePrefabForComponent<IBattleUnit>(prefab.GameObject);
            _operations.Add((unit, reference));

            if (_initiativeQueue != null)
            {
                if (lastInitiative)
                    _initiativeQueue.AddUnitInTheEnd(unit);
                else 
                    await _initiativeQueue.AddUnit(unit, ct);
            }

            return unit;
        }

        public void Summon(
            AssetReferenceObject<IBattleUnit> reference,
            bool lastInitiative,
            Action<IBattleUnit> onSuccess = null)
        {
            if (_scope.TryGetToken(out var ct))
                SummonAsync(reference, lastInitiative, ct).ContinueWith(x => onSuccess?.Invoke(x));
        }

        public void DestroyBy(IBattleUnit unit)
        {
            for (int i = 0; i < _operations.Count; i++)
            {
                var (u, reference) = _operations[i];
                if (u == unit)
                {
                    Object.Destroy(u.GameObject);
                    reference.ReleaseIfValid();
                    _operations.RemoveAt(i);
                    return;
                }
            }
        }

        public void DestroyBy(AssetReferenceObject<IBattleUnit> reference)
        {
            for (int i = 0; i < _operations.Count; i++)
            {
                var (unit, r) = _operations[i];
                if (r == reference)
                {
                    Object.Destroy(unit.GameObject);
                    r.ReleaseIfValid();
                    _operations.RemoveAt(i);
                    return;
                }
            }
        }

        public void DestroyAll()
        {
            foreach (var (unit, reference) in _operations)
            {
                Object.Destroy(unit.GameObject);
                reference.ReleaseIfValid();
            }
            
            _operations.Clear();
        }
    }
}