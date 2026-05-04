using UnityEngine;

namespace Game.Scripts.Characters.ModularCharacters
{
    [CreateAssetMenu(menuName = "Data/Animations/Attack")]
    public class AttackAnimationData : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float HitTiming { get; private set; }
        [field: SerializeField] public float HitDistance { get; private set; }
    }
}