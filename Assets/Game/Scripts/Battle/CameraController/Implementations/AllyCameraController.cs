using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Battle.CameraController.Data;
using Game.Scripts.Battle.UnitsTeam;
using UnityEngine;

namespace Game.Scripts.Battle.CameraController.Implementations
{
    public class AllyCameraController : CameraController
    {
        public override void SetTargets(
            IEnumerable<Transform> follow,
            IEnumerable<Transform> targets,
            bool saveCommand,
            ZoomType zoomType = ZoomType.Default)
        {
            var followUnit = follow.FirstOrDefault();
            var targetUnit = targets.FirstOrDefault();

            if (followUnit == null)
            {
                base.SetTargets(follow, targets, saveCommand, zoomType);
                return;
            }
            
            var followTeam = UnitTeamsUtils.GetTeam(followUnit.GetComponent<ITeamUnit>());

            if (targetUnit != null &&
                followTeam is UnitTeams.Enemy)
            {
                base.SetTargets(targets, follow, saveCommand, zoomType);
                return;
            }
            
            base.SetTargets(follow, targets, saveCommand, zoomType);
        }
    }
}