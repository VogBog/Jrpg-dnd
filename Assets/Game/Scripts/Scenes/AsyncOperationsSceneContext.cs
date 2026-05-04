using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.Scenes
{
    public class AsyncOperationsSceneContext : IAsyncOperationContext, IAsyncOperationScope
    {
        private readonly List<UniTask> _tasks = new(16);
        private CancellationTokenSource _cts;
        private bool _active = false;
        
        public bool IsActive => _active;

        public bool TryGetToken(out CancellationToken ct)
        {
            if (!_active)
                return false;
            
            ct = _cts.Token;
            return true;
        }

        public void FireAndForget(UniTask task)
        {
            try
            {
                _tasks.Add(task);
                WaitForTaskAndUnregister(task, false).Forget();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void FireAndForgetDelayed(UniTask task)
        {
            try
            {
                _tasks.Add(task);
                WaitForTaskAndUnregister(task, true).Forget();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public bool TryRegister(UniTask task)
        {
            if (!_active)
                return false;
            
            _tasks.Add(task);
            return true;
        }

        public void UnRegister(UniTask task)
        {
            _tasks.Remove(task);
        }

        public async UniTask CancelOperationsAsync()
        {
            _active = false;
            _cts.Cancel();

            while (_tasks.Count > 0)
            {
                await UniTask.NextFrame();
            }
            
            _cts.Dispose();
            _cts = null;
        }

        public void CancelOperations(Action onComplete)
        {
            CancelOperationsAsync().ContinueWith(onComplete);
        }

        public void OpenContext()
        {
            if (_active || _cts != null)
                throw new Exception("Cannot open context twice");

            _cts = new();
            _active = true;
        }
        
        private async UniTask WaitForTaskAndUnregister(UniTask task, bool delayed)
        {
            if (delayed)
                await UniTask.Yield();
            
            await task;
            UnRegister(task);
        }
    }
}