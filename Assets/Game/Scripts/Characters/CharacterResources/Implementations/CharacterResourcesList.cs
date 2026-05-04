using System;
using Game.Scripts.Characters.CharacterResources.DefaultResources;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Characters.CharacterResources.Implementations
{
    [CreateAssetMenu(menuName = "Data/Character/Resource Data List")]
    public class CharacterResourcesList : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceT<CharacterResourceData>[] Resources { get; private set; }
        [field: SerializeField] public MarkedData[] MarkedResources { get; private set; }

        [Serializable]
        public struct MarkedData
        {
            public int MarkValue;
            public AssetReferenceT<CharacterMarkedResourceData> Resource;
        }
    }
}