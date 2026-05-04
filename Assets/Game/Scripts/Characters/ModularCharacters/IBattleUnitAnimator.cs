namespace Game.Scripts.Characters.ModularCharacters
{
    public interface IBattleUnitAnimator
    {
        public void Dodge();
        public void Hit();
        public void Die();
        public AttackAnimationData Attack();
        
        public void SetIdleId(IdleAnimationData id);
        public void SetAttackId(AttackAnimationData data);
        public AttackAnimationData GetAttackData();
    }
}