using Game.Scripts.Battle;
using Game.Scripts.Characters.ModularCharacters;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Contexts
{
    [RequireComponent(typeof(IBattleUnit))]
    public class BattleUnitInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var battleUnit = GetComponent<IBattleUnit>();
            Container.Bind<IBattleUnit>().FromInstance(battleUnit).AsSingle();

            var animator = GetComponentInChildren<IBattleUnitAnimator>();
            Container.Bind<IBattleUnitAnimator>().FromInstance(animator).AsSingle();
        }
    }
}