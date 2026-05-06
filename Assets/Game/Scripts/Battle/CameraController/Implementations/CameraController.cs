using System;
using System.Collections.Generic;
using Game.Scripts.Battle.CameraController.Data;
using Game.Scripts.Battle.CameraController.Interfaces;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Scripts.Battle.CameraController.Implementations
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private CinemachineGroupFraming _framing;
        [SerializeField] private CinemachineTargetGroup _targetsGroup;
        [SerializeField] private ZoomTypeValue[] _zoomValues;

        private readonly Stack<SetCameraTargetsCommand> _commands = new();
        private SetCameraTargetsCommand _lastCommand;

        public int SavedCommandsCount => _commands.Count + (_lastCommand.Follow != null ? 1 : 0);
        public IEnumerable<Transform> Follow => 
            _lastCommand.Targets != null ? _lastCommand.Targets : Array.Empty<Transform>();
        public IEnumerable<Transform> Targets => 
            _lastCommand.Targets != null ? _lastCommand.Targets : Array.Empty<Transform>();

        public virtual void SetTargets(
            Transform follow,
            IEnumerable<Transform> targets,
            bool saveCommand,
            ZoomType zoomType = ZoomType.Default)
            => SetTargets(new[] { follow }, targets, saveCommand, zoomType);

        public virtual void SetTargets(
            Transform follow,
            bool saveCommand,
            ZoomType zoomType = ZoomType.Default,
            params Transform[] targets)
            => SetTargets(new [] { follow }, targets, saveCommand, zoomType);
        
        public virtual void SetTargets(
            IEnumerable<Transform> follow,
            IEnumerable<Transform> targets,
            bool saveCommand,
            ZoomType zoomType = ZoomType.Default)
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

            foreach (var zoomValue in _zoomValues)
            {
                if (zoomType == zoomValue.Type)
                {
                    _framing.FramingSize = zoomValue.Value;
                    break;
                }
            }

            if (saveCommand)
            {
                if (_lastCommand.Follow != null && _lastCommand.Targets != null)
                    _commands.Push(_lastCommand);

                var followCopy = ListPool<Transform>.Get();
                followCopy.AddRange(follow);
                var targetsCopy = ListPool<Transform>.Get();
                targetsCopy.AddRange(targets);
                
                _lastCommand = new SetCameraTargetsCommand(followCopy, targetsCopy, zoomType);
            }
        }

        public virtual void ReturnToBack()
        {
            if (_commands.Count == 0)
                return;
            
            var command = _commands.Pop();

            if (_lastCommand is { Follow: not null, Targets: not null })
            {
                ListPool<Transform>.Release(_lastCommand.Follow);
                ListPool<Transform>.Release(_lastCommand.Targets);
            }
            
            _lastCommand = command;
            SetTargets(command.Follow, command.Targets, false, command.Zoom);
        }
    }
}