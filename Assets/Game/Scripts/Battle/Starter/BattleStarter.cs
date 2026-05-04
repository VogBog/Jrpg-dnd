using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.DataStorage.FreqDataStorage;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.Starter
{
    public class BattleStarter
    {
        private IInitiativeQueue _initiativeQueue;
        private IBattleUnitSummoner _summoner;
        private IDataStorage _dataStorage;
        
        [Inject]
        public void Construct(IInitiativeQueue queue, IBattleUnitSummoner summoner, IDataStorage dataStorage)
        {
            _initiativeQueue = queue;
            _summoner = summoner;
            _dataStorage = dataStorage;
        }

        public void Start(
            AssetReferenceObject<IBattleUnit> playerReference1,
            AssetReferenceObject<IBattleUnit> playerReference2,
            AssetReferenceObject<IBattleUnit> playerReference3,
            AssetReferenceObject<IBattleUnit> enemyReference,
            AssetReferenceObject<IBattleUnit> enemyReference2)
        {
            StartAsync(playerReference1, playerReference2, playerReference3, enemyReference, enemyReference2).Forget();
        }

        private async UniTask StartAsync(
            AssetReferenceObject<IBattleUnit> playerReference1,
            AssetReferenceObject<IBattleUnit> playerReference2,
            AssetReferenceObject<IBattleUnit> playerReference3,
            AssetReferenceObject<IBattleUnit> enemyReference,
            AssetReferenceObject<IBattleUnit> enemyReference2)
        {
            await UniTask.WaitForSeconds(0.1f);
            while (_dataStorage.Loading)
                await UniTask.WaitForSeconds(0.2f);
            
            await _summoner.SummonAsync(playerReference1.GetCopy(), false, CancellationToken.None);
            await _summoner.SummonAsync(playerReference2.GetCopy(), false, CancellationToken.None);
            await _summoner.SummonAsync(playerReference3.GetCopy(), false, CancellationToken.None);

            for (int i = 0; i < 5; i++)
            {
                await _summoner.SummonAsync(enemyReference.GetCopy(), false, CancellationToken.None);
            }

            await _summoner.SummonAsync(enemyReference2.GetCopy(), false, CancellationToken.None);

            await UniTask.WaitForSeconds(0.1f);

            while (_dataStorage.Loading)
                await UniTask.WaitForSeconds(0.2f);

            Debug.Log("START FIRST TURN");
            _initiativeQueue.StartFirstTurn();
            Debug.Log("FIRST TURN STARTED");
        }
    }
}