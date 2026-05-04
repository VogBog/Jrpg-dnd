using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.TargetChooser.Data;
using Game.Scripts.Battle.TargetChooser.Interfaces;
using Game.Scripts.Battle.UnitsTeam;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.TargetChooser.Implementations
{
    public class EnemyAITargetChooser : ITargetChooser
    {
        private IInitiativeQueue _queue;
        
        [Inject]
        private void Construct(IInitiativeQueue queue)
        {
            _queue = queue;
        }
        
        public async UniTask<List<IBattleUnit>> Choose(ChooseTargetCommand command, CancellationToken ct)
        {
            var mask = command.Filter.GetMaskForEnemy();
            
            var targets = _queue.Queue
                .Where(x => mask.HasTeam(UnitTeamsUtils.GetTeam(x.Unit)))
                .ToList();

            if (targets.Count == 0)
            {
                return null;
            }

            var result = new List<IBattleUnit>();
            for (int i = 0; i < command.MaxCount && targets.Count > 0; i++)
            {
                int randIndex = Random.Range(0, targets.Count);
                result.Add(targets[randIndex].Unit);
                targets.RemoveAt(randIndex);
            }

            return result;
        }
    }
}