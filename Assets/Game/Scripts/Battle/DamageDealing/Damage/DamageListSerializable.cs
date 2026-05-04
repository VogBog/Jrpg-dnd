using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.DataStorage.FreqDataStorage;
using UnityEngine;

namespace Game.Scripts.Battle.DamageDealing.Damage
{
    [Serializable]
    public class DamageListSerializable
    {
        [SerializeField] private List<DamageValueSerializable> _values;

        public DamageList Values;

        public async UniTask<DamageList> Map(IDataStorage storage, CancellationToken ct)
        {
            var list = new List<DamageValue>();
            foreach (var value in _values)
            {
                var damageType = await storage.LoadT(value.DamageType);
                list.Add(new DamageValue(value.Dices, damageType, value.DamageBonus));
                
                ct.ThrowIfCancellationRequested();
            }

            Values = new DamageList(list);
            return Values;
        }
    }
}