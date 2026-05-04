using UnityEngine;

namespace Game.Scripts.Battle.BattleAnimations.Implementations
{
    public class BaseSaveThrowEffect : MonoBehaviour
    {
        [field: SerializeField] public ParticleSystem ParticleSystem { get; private set; }
    }
}