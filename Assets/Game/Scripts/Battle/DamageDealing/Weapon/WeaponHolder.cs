using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.DataStorage.FreqDataStorage;
using Game.Scripts.Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Battle.DamageDealing.Weapon
{
    public class WeaponHolder : MonoBehaviour
    {
        private IAsyncOperationScope _scope;
        private IDataStorage _storage;
        private AssetReferenceT<WeaponItem> _reference;
        
        [SerializeField] private AssetReferenceT<WeaponItem> _initWeaponReference;
        
        public readonly SimpleAttribute DamageBonus = new();
        public readonly SimpleAttribute AttackBonus = new();

        public event Action<WeaponHolder> Changed; 
        
        public WeaponItem Item { get; private set; }
        public IBattleUnit Owner { get; private set; }

        [Inject]
        private void Construct(IBattleUnit owner, IAsyncOperationScope scope, IDataStorage dataStorage)
        {
            Owner = owner;
            _scope = scope;
            _storage = dataStorage;
        }

        public async UniTask SetWeaponAsync(AssetReferenceT<WeaponItem> weaponReference, CancellationToken ct)
        {
            if (_reference != null)
            {
                Item = null;
                if (_reference.IsValid())
                    _reference.ReleaseAsset();
            }

            _reference = weaponReference;
            Item = await _reference.LoadAssetAsync();
            
            ct.ThrowIfCancellationRequested();
            
            await Item.Map(_storage, ct);
            
            Changed?.Invoke(this);
        }

        public void SetWeaponForget(AssetReferenceT<WeaponItem> weaponReference)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(SetWeaponAsync(weaponReference, ct));
        }

        private void Awake()
        {
            if (_initWeaponReference == null)
                throw new NullReferenceException("WeaponHolder Init Weapon Reference is null");

            AttackBonus.Changed += _ => Changed?.Invoke(this);
            DamageBonus.Changed += _ => Changed?.Invoke(this);
            
            SetWeaponForget(_initWeaponReference);
        }

        private void OnDestroy()
        {
            if (_reference != null && _reference.IsValid())
                _reference.ReleaseAsset();
        }
    }
}