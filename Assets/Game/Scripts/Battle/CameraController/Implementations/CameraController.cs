using System;
using System.Collections.Generic;
using Game.Scripts.Battle.CameraController.Data;
using Game.Scripts.Battle.CameraController.Interfaces;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Scripts.Battle.CameraController.Implementations
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private CinemachineTargetGroup _targetsGroup;

        private readonly Stack<SetCameraTargetsCommand> _commands = new();
        private SetCameraTargetsCommand _lastCommand;

        public int SavedCommandsCount => _commands.Count + (_lastCommand.Follow != null ? 1 : 0);
        public IEnumerable<Transform> Follow => _lastCommand.Targets ?? Array.Empty<Transform>();
        public IEnumerable<Transform> Targets => _lastCommand.Targets ?? Array.Empty<Transform>();

        public void SetTargets(
            Transform follow,
            IEnumerable<Transform> targets,
            bool saveCommand)
            => SetTargets(new[] { follow }, targets, saveCommand);

        public void SetTargets(
            Transform follow,
            bool saveCommand,
            params Transform[] targets)
            => SetTargets(new [] { follow }, targets, saveCommand);
        
        public void SetTargets(
            IEnumerable<Transform> follow,
            IEnumerable<Transform> targets,
            bool saveCommand)
        {
            _targetsGroup.Targets.Clear();

            foreach (var target in follow)
            {
                _targetsGroup.AddMember(target, 3f, 1f);
            }
            
            foreach (var target in targets)
            {
                _targetsGroup.AddMember(target, 1f, 1f);
            }

            if (saveCommand)
            {
                if (_lastCommand.Follow != null)
                    _commands.Push(_lastCommand);
                
                _lastCommand = new SetCameraTargetsCommand(follow, targets);
            }
        }

        public void ReturnToBack()
        {
            if (_commands.Count == 0)
                return;
            
            var command = _commands.Pop();
            _lastCommand = command;
            
            SetTargets(command.Follow, command.Targets, false);
        }
    }
}