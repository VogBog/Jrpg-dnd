using UnityEngine;

namespace Game.Scripts.Characters.ModularCharacters
{
    [CreateAssetMenu(menuName = "Data/Animations/Idle")]
    public class IdleAnimationData : ScriptableObject
    {
        [field: SerializeField] public int Index { get; private set; }
    }
}