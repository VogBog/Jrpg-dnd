using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Battle.CameraController.Data
{
    public readonly struct SetCameraTargetsCommand
    {
        public readonly List<Transform> Follow;
        public readonly List<Transform> Targets;

        public SetCameraTargetsCommand(List<Transform> follow, List<Transform> targets)
        {
            Follow = follow;
            Targets = targets;
        }
    }
}