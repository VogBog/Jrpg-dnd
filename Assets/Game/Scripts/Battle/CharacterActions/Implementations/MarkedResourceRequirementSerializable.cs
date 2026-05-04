using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Characters.CharacterResources.DefaultResources;
using Game.Scripts.DataStorage.FreqDataStorage;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Battle.CharacterActions.Implementations
{
    [Serializable]
    public class MarkedResourceRequirementSerializable
    {
        [field: SerializeField] public AssetReferenceT<CharacterMarkedResourceData> DataReference { get; private set; }
        
        public CharacterMarkedResourceData Data { get; private set; }
        public int MarkValue;

        public async UniTask<CharacterMarkedResourceData> Map(IDataStorage storage)
        {
            Data = await storage.LoadT(DataReference);
            return Data;
        }
    }
}