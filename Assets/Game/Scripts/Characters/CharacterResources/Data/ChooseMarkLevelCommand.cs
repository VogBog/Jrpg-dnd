using Game.Scripts.Characters.CharacterResources.DefaultResources;
using Game.Scripts.Characters.CharacterResources.Interfaces;

namespace Game.Scripts.Characters.CharacterResources.Data
{
    public struct ChooseMarkLevelCommand
    {
        public ICharacterResourcesHolder Holder;
        public CharacterMarkedResourceData Data;
        public int MinLevel;

        public ChooseMarkLevelCommand(ICharacterResourcesHolder holder, CharacterMarkedResourceData data, int minLevel)
        {
            Holder = holder;
            Data = data;
            MinLevel = minLevel;
        }
    }
}