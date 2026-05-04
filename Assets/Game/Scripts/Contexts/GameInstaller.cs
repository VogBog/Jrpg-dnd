using Game.Scripts.DataStorage.FreqDataStorage;
using Game.Scripts.Events.Implementations;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Pool.Implementations;
using Game.Scripts.Pool.Interfaces;
using Game.Scripts.Rolls.Implementations;
using Game.Scripts.Rolls.Interfaces;
using Game.Scripts.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Contexts
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private DataStorage.FreqDataStorage.DataStorage _dataStorage;
        
        public override void InstallBindings()
        {
            Container.Bind<ISceneChanger>().To<SceneChanger>().AsSingle();
            Container.Bind<IDiceRoller>().To<DiceRoller>().AsSingle();
            Container.Bind<ICheckRoller>().To<CheckRoller>().AsSingle();
            Container.Bind<IEventPool>().To<EventPool>().AsSingle();
            Container.Bind<IEventBus>().To<EventBus>().AsSingle();
            Container.Bind<IObjectPool>().To<ObjectPool>().AsSingle();
            Container.Bind<IDataStorage>().FromInstance(_dataStorage).AsSingle();

            var sceneScope = new AsyncOperationsSceneContext();
            Container.Bind<IAsyncOperationContext>().FromInstance(sceneScope).AsSingle();
            Container.Bind<IAsyncOperationScope>().FromInstance(sceneScope).AsSingle();
            
            sceneScope.OpenContext();
        }
    }
}