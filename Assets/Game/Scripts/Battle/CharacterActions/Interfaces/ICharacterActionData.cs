using UnityEngine;

namespace Game.Scripts.Battle.CharacterActions.Interfaces
{
    public interface ICharacterActionData
    {
        string Name { get; }
        string Description { get; }
        Sprite Icon { get; }
    }
}