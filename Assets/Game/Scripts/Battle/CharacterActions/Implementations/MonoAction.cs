using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Characters.CharacterResources.DefaultResources;
using Game.Scripts.Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Battle.CharacterActions.Implementations
{
    public abstract class MonoAction : MonoBehaviour, ICharacterAction, ICharacterActionData
    {
        [SerializeField] private AssetReferenceSprite _spriteReference;
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        public Sprite Icon { get; private set; }
        
        public IBattleUnit Owner { get; private set; }
        
        public IEnumerable<CharacterResourceData> UseResources { get; protected set; }
        public IEnumerable<MarkedResourceRequirementSerializable> UseMarkedResources { get; protected set; }

        [Inject]
        private void Construct(IBattleUnit owner, IAsyncOperationScope scope)
        {
            Owner = owner;
            if (scope.TryGetToken(out _))
                scope.FireAndForget(Map());
        }

        public abstract bool CanUse();

        public async UniTask<Sprite> Map()
        {
            Icon = await _spriteReference.LoadAssetAsync();
            return Icon;
        }

        protected virtual void OnDestroy()
        {
            if (_spriteReference.IsValid())
                _spriteReference.ReleaseAsset();
        }
    }
}