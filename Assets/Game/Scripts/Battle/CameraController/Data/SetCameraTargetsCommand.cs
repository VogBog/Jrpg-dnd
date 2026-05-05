using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Battle.CameraController.Data
{
    public struct SetCameraTargetsCommand
    {
        public IEnumerable<Transform> Follow;
        public IEnumerable<Transform> Targets;

        public SetCameraTargetsCommand(IEnumerable<Transform> follow, IEnumerable<Transform> targets)
        {
            Follow = follow;
            Targets = targets;
        }
    }
}