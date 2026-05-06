using System.Collections.Generic;
using Game.Scripts.Battle.CameraController.Data;
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
            bool saveCommand,
            ZoomType zoomType = ZoomType.Default);

        void SetTargets(
            Transform follow,
            bool saveCommand,
            ZoomType zoomType = ZoomType.Default,
            params Transform[] targets);
        
        void SetTargets(
            IEnumerable<Transform> follow,
            IEnumerable<Transform> targets,
            bool saveCommand,
            ZoomType zoomType = ZoomType.Default);

        void ReturnToBack();
    }
}