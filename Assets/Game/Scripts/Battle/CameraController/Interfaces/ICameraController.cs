using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Battle.CameraController.Interfaces
{
    public interface ICameraController
    {
        int SavedCommandsCount { get; }
        IEnumerable<Transform> Follow { get; }
        IEnumerable<Transform> Targets { get; }
        
        void SetTargets(
            Transform follow,
            IEnumerable<Transform> targets,
            bool saveCommand);

        void SetTargets(
            Transform follow,
            bool saveCommand,
            params Transform[] targets);
        
        void SetTargets(
            IEnumerable<Transform> follow,
            IEnumerable<Transform> targets,
            bool saveCommand);

        void ReturnToBack();
    }
}