using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.DamageDealing.Weapon;
using Game.Scripts.DataStorage.FreqDataStorage;
using Game.Scripts.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Characters.ModularCharacters
{
    [RequireComponent(typeof(CharacterAnimatorController))]
    public class ModularCharacter : MonoBehaviour
    {
        [SerializeField] private Character _character;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        
        private IAsyncOperationScope _scope;
        private IDataStorage _dataStorage;
        private WeaponHolder _weaponHolder;
        
        private GameObject _model;
        
        [Inject]
        private void Construct(IAsyncOperationScope scope, IDataStorage dataStorage)
        {
            _scope = scope;
            _dataStorage = dataStorage;
        }
        
        public async UniTask SetWeaponAsync(WeaponItem weaponItem, CancellationToken ct)
        {
            if (_model != null)
            {
                Destroy(_model);
            }

            var modelReference = weaponItem.Model;
            if (modelReference != null && modelReference.RuntimeKeyIsValid())
            {
                var prefab = await _dataStorage.LoadT(modelReference);
                _model = Instantiate(prefab, _rightHand);
            }

            if (!TryGetComponent(out IBattleUnitAnimator animator))
                return;

            var newAttack = await _dataStorage.LoadT(weaponItem.AttackAnimation);
            animator.SetAttackId(newAttack);

            var newIdle = await _dataStorage.LoadT(weaponItem.IdleAnimation);
            animator.SetIdleId(newIdle);
        }

        private void Awake()
        {
            _weaponHolder = _character.GetComponent<WeaponHolder>();
            if (_weaponHolder != null)
                _weaponHolder.Changed += OnWeaponChanged;
        }

        private void OnDestroy()
        {
            if (_weaponHolder != null)
                _weaponHolder.Changed -= OnWeaponChanged;
        }

        private void OnWeaponChanged(WeaponHolder holder)
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForgetDelayed(SetWeaponAsync(holder.Item, ct));
        }
    }
}