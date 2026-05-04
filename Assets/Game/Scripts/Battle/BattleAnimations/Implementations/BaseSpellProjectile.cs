using UnityEngine;

namespace Game.Scripts.Battle.BattleAnimations.Implementations
{
    public class BaseSpellProjectile : MonoBehaviour
    {
        [field: SerializeField] public ParticleSystem ParticleSystem { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }

        private Vector3 _direction;

        public void Throw(Vector3 point)
        {
            _direction = Vector3.Normalize(point - transform.position);
        }

        private void Update()
        {
            transform.position += Time.deltaTime * Speed * _direction;
        }
    }
}