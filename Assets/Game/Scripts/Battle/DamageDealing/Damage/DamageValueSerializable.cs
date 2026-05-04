using System;
using Game.Scripts.Rolls.Data;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Battle.DamageDealing.Damage
{
    [Serializable]
    public class DamageValueSerializable
    {
        public DiceParameters Dices;
        public AssetReferenceT<DamageType> DamageType;
        public int DamageBonus;
    }
}