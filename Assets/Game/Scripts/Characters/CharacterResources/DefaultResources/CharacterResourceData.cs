using UnityEngine;

namespace Game.Scripts.Characters.CharacterResources.DefaultResources
{
    [CreateAssetMenu(menuName = "Data/Character/Resource Data")]
    public class CharacterResourceData : ScriptableObject
    {
        [field: SerializeField] public bool IsRestoreInNextTurn { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}