using Game.Scripts.Characters.CharacterResources.Interfaces;

namespace Game.Scripts.Characters.CharacterResources.DefaultResources
{
    public class BaseMarkedResource : BaseResource, ICharacterMarkedResource
    {
        public CharacterMarkedResourceData MarkedData { get; set; }
        
        public int MarkValue { get; set; }
    }
}