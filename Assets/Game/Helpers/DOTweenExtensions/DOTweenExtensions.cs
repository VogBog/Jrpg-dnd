using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace Game.Helpers.DOTweenExtensions
{
    public static class DOTweenExtensions
    {
        public static UniTask ToUniTask<T1, T2, T3>(this TweenerCore<T1, T2, T3> core, CancellationToken ct)
        where T3 : struct, IPlugOptions
        {
            var valueRef = new ValueRef<bool>
            {
                Value = false
            };

            core.OnComplete(() => valueRef.Value = true);

            return UniTask.WaitUntil(() => valueRef.Value, cancellationToken: ct);
        }
    }
}