using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.DamageDealing;
using Game.Scripts.Battle.DamageDealing.Armor;
using UnityEngine;

namespace Game.Scripts.Battle.CharacterActions.Actions.FighterActions
{
    public class DefenseFightingStyle : BaseCharacterAction, IPassiveEffect
    {
        [SerializeField] private int _addArmor;
        
        protected override async UniTask<bool> BeforeUse(CancellationToken ct)
        {
            return true;
        }

        protected override UniTask OnUse(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        protected override void ClearDataAfterUse()
        {
            
        }

        public void OnAdded(IBattleUnit owner)
        {
            if (owner.GameObject.TryGetComponent(out Armor armor))
                armor.Bonus.AddModifier(new SimpleStatModifier(nameof(DefenseFightingStyle), _addArmor));
        }

        public void OnRemoved()
        {
            if (Owner.GameObject.TryGetComponent(out Armor armor))
                armor.Bonus.RemoveModifier(nameof(DefenseFightingStyle));
        }
        
        protected override List<IBattleUnit> GetTargets() => new List<IBattleUnit> { Owner };
    }
}