using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Battle.UnitsTeam;
using UnityEngine;

namespace Game.Scripts.Battle.CameraController.Implementations
{
    public class AllyCameraController : CameraController
    {
        public override void SetTargets(IEnumerable<Transform> follow, IEnumerable<Transform> targets, bool saveCommand)
        {
            var followUnit = follow.FirstOrDefault();
            var targetUnit = targets.FirstOrDefault();
            var followTeam = UnitTeamsUtils.GetTeam(followUnit.GetComponent<ITeamUnit>());

            if (followUnit != null && targetUnit != null &&
                followTeam is UnitTeams.Enemy)
            {
                base.SetTargets(targets, follow, saveCommand);
                return;
            }
            
            base.SetTargets(follow, targets, saveCommand);
        }
    }
}