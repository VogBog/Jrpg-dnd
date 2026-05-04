using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Scenes
{
    public interface IAsyncOperationScope
    {
        public bool TryGetToken(out CancellationToken ct);
        public void FireAndForget(UniTask task);
        public void FireAndForgetDelayed(UniTask task);
        public bool TryRegister(UniTask task);
        public void UnRegister(UniTask task);
    }
}