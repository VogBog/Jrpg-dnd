using System;
using UnityEngine;

namespace Game.Scripts.Characters.CharacterResources.DefaultResources
{
    [CreateAssetMenu(menuName = "Data/Character/Marked Resource Data")]
    public class CharacterMarkedResourceData : CharacterResourceData
    {
        [field: SerializeField] public Variant[] Variants { get; private set; }



        [Serializable]
        public struct Variant
        {
            [field: SerializeField] public int MarkValue { get; private set; }
            [field: SerializeField] public Sprite Icon { get; private set; }
        }
    }
}