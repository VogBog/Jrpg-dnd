using Cysharp.Threading.Tasks;

namespace Game.Scripts.Scenes
{
    public interface ISceneChanger
    {
        public void LoadScene(string sceneName);
        public void LoadScene(int sceneIndex);
        public UniTask LoadSceneAsync(string sceneName);
        public UniTask LoadSceneAsync(int sceneIndex);
    }
}