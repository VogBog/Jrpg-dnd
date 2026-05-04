using Game.Helpers.AddressablesHelpers;
using Game.Helpers.ZenjectHelpers;
using Game.Scripts.Battle;
using Game.Scripts.Battle.BattleAnimations.Implementations;
using Game.Scripts.Battle.BattleAnimations.Interfaces;
using Game.Scripts.Battle.CharacterActions.Spells.Templates;
using Game.Scripts.Battle.CharacterAnalyze.Implementations;
using Game.Scripts.Battle.CharacterAnalyze.Interfaces;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.PopupWindow.Implementation;
using Game.Scripts.Battle.PopupWindow.Interfaces;
using Game.Scripts.Battle.Starter;
using Game.Scripts.Battle.TargetChooser.Implementations;
using Game.Scripts.Battle.TargetChooser.Interfaces;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Characters.CharacterResources.Implementations;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Contexts
{
    //TODO - Change for normal spawn logic
    public class BattleSceneInstaller : MonoInstaller
    {
        [SerializeField] private AssetReferenceObject<IBattleUnit> _playerReference1;
        [SerializeField] private AssetReferenceObject<IBattleUnit> _playerReference2;
        [SerializeField] private AssetReferenceObject<IBattleUnit> _playerReference3;
        [SerializeField] private AssetReferenceObject<IBattleUnit> _enemyReference;
        [SerializeField] private AssetReferenceObject<IBattleUnit> _enemyReference2;
        
        [SerializeField] private UnitBattlePositioning _unitBattlePositioning;
        [SerializeField] private PlayerMouseTargetChooser _playerMouseTargetChooser;
        [SerializeField] private MouseCharacterSelector _characterSelector;
        [SerializeField] private BattleAnimationsPlayer _battleAnimationsPlayer;
        [SerializeField] private PlayerPopupWindowCreator _popupWindow;
        [SerializeField] private BaseSpellAnimationsPlayer _baseSpellAnimations;
        
        private readonly BattleStarter _battleStarter = new ();
        
        public override void InstallBindings()
        {
            Container.Bind<IInitiativeQueue>().To<InitiativeQueue>().AsSingle();
            Container.Bind<IUnitBattlePositioning>().FromInstance(_unitBattlePositioning).AsSingle();
            Container.Bind<IBattleUnitSummoner>().To<BattleUnitSummoner>().AsSingle();
            Container.Bind<ICharacterSelector>().FromInstance(_characterSelector).AsSingle();
            Container.Bind<IBattleAnimationsPlayer>().FromInstance(_battleAnimationsPlayer).AsSingle();
            Container.Bind<BaseSpellAnimationsPlayer>().FromInstance(_baseSpellAnimations).AsSingle();

            BindTargetChoose();
            BindMarkLevelChooser();
            BindPopupWindowCreator();

            Container.Bind<BattleStarter>().FromInstance(_battleStarter).AsSingle();
        }

        public override void Start()
        {
            base.Start();
            Container.Inject(_battleStarter);
            _battleStarter.Start(_playerReference1, _playerReference2, _playerReference3, _enemyReference, _enemyReference2);
        }

        private void BindTargetChoose()
        {
            Container
                .Bind(typeof(ITargetChooser), typeof(IPlayerTargetChooser))
                .FromInstance(_playerMouseTargetChooser)
                .AsSingle()
                .WhenNotAI();
            
            Container.Bind<ITargetChooser>().To<EnemyAITargetChooser>().AsSingle().WhenAIControls();
        }

        private void BindMarkLevelChooser()
        {
            var markLevelChooser = new PlayerResourceMarkLevelChooser();
            Container.Bind(
                    typeof(ICharacterResourceMarkLevelChooser),
                    typeof(IPlayerResourceMarkLevelChooser))
                .FromInstance(markLevelChooser)
                .AsSingle()
                .WhenNotAI();

            Container
                .Bind<ICharacterResourceMarkLevelChooser>()
                .To<AIResourceMarkLevelChooser>()
                .AsSingle()
                .WhenAIControls();
        }

        private void BindPopupWindowCreator()
        {
            Container.Bind<IPopupWindowCreator>()
                .FromInstance(_popupWindow)
                .AsSingle()
                .WhenNotAI();

            Container.Bind<IPopupWindowCreator>()
                .To<EnemyPopupWindowCreator>()
                .AsSingle()
                .WhenAIControls();
        }
    }
}