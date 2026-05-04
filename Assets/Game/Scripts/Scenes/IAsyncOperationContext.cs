using System;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Scenes
{
    public interface IAsyncOperationContext
    {
        bool IsActive { get; }

        public UniTask CancelOperationsAsync();
        public void CancelOperations(Action onComplete);
        public void OpenContext();
    }
}