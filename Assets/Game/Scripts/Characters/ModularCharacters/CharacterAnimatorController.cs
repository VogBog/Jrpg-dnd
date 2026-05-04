using UnityEngine;

namespace Game.Scripts.Characters.ModularCharacters
{
    public class CharacterAnimatorController : MonoBehaviour, IBattleUnitAnimator
    {
        [SerializeField] private Animator _animator;
        
        private IdleAnimationData _idle;
        private AttackAnimationData _attack;
        
        private static readonly int DodgeParam = Animator.StringToHash("Dodge");
        private static readonly int HitParam = Animator.StringToHash("Hit");
        private static readonly int DeathParam = Animator.StringToHash("Death");
        private static readonly int IdleIdParam = Animator.StringToHash("IdleId");
        private static readonly int AttackParam = Animator.StringToHash("Attack");
        private static readonly int AttackIdParam = Animator.StringToHash("AttackId");

        public void Dodge() => _animator.SetTrigger(DodgeParam);
        public void Hit() => _animator.SetTrigger(HitParam);
        public void Die() => _animator.SetTrigger(DeathParam);

        public AttackAnimationData Attack()
        {
            _animator.SetTrigger(AttackParam);
            return _attack;
        }

        public void SetIdleId(IdleAnimationData data)
        {
            _idle = data;
            _animator.SetInteger(IdleIdParam, data.Index);
        }

        public void SetAttackId(AttackAnimationData data)
        {
            _attack = data;
            _animator.SetInteger(AttackIdParam, data.Id);
        }

        public AttackAnimationData GetAttackData() => _attack;
    }
}