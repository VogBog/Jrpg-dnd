using Game.Scripts.Characters.CharacterResources.DefaultResources;

namespace Game.Scripts.Characters.CharacterResources.Interfaces
{
    public interface ICharacterMarkedResource : ICharacterResource
    {
        CharacterMarkedResourceData MarkedData { get; set; }
        
        int MarkValue { get; set; }
    }
}