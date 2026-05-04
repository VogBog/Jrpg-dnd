using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Helpers.AddressablesHelpers;

namespace Game.Scripts.Battle.Starter
{
    public interface IBattleUnitSummoner
    {
        UniTask<IBattleUnit> SummonAsync(
            AssetReferenceObject<IBattleUnit> reference,
            bool lastInitiative,
            CancellationToken ct);
        
        void Summon(
            AssetReferenceObject<IBattleUnit> reference,
            bool lastInitiative,
            Action<IBattleUnit> onSuccess = null);
        
        void DestroyBy(IBattleUnit unit);
        void DestroyBy(AssetReferenceObject<IBattleUnit> reference);
        void DestroyAll();
    }
}