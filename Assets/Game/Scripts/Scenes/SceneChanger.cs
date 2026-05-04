using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Scripts.Scenes
{
    public class SceneChanger : ISceneChanger
    {
        private IAsyncOperationContext _opContext;
        
        [Inject]
        private void Construct(IAsyncOperationContext opContext)
        {
            _opContext = opContext;
        }

        public void LoadScene(string sceneName)
        {
            LoadSceneAsync(sceneName).Forget();
        }

        public void LoadScene(int sceneIndex)
        {
            LoadSceneAsync(sceneIndex).Forget();
        }

        public UniTask LoadSceneAsync(string sceneName)
        {
            return LoadSceneAsyncInternal(() => SceneManager.LoadSceneAsync(sceneName));
        }

        public UniTask LoadSceneAsync(int sceneIndex)
        {
            return LoadSceneAsyncInternal(() => SceneManager.LoadSceneAsync(sceneIndex));
        }

        private async UniTask LoadSceneAsyncInternal(Func<AsyncOperation> loadSceneOperation)
        {
            await _opContext.CancelOperationsAsync();
            await loadSceneOperation.Invoke();
            _opContext.OpenContext();
        }
    }
}