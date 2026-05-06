using System;
using System.Linq;
using Game.Scripts.Battle.CameraController.Data;
using Game.Scripts.Battle.CameraController.Interfaces;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.TargetChooser.Data;
using Game.Scripts.Battle.TargetChooser.Interfaces;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Events.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.CameraController.Implementations
{
    public class CameraControllerAdditionalAnimations : MonoBehaviour
    {
        [Inject] private IInitiativeQueue _queue;
        [Inject] private IEventBus _bus;
        [Inject] private ICameraController _cameraController;
        [Inject] private IPlayerTargetChooser _targetChooser;

        private void OnEnable()
        {
            _queue.UnitAdded += OnUnitAddedToInitiative;
            _targetChooser.SelectionStarted += OnSelectionStarted;
            _targetChooser.SelectionEnded += OnSelectionEnded;
            _bus.Subscribe<TurnStartingEvent>(OnTurnStarted);
        }

        private void OnDisable()
        {
            _queue.UnitAdded -= OnUnitAddedToInitiative;
            _targetChooser.SelectionStarted -= OnSelectionStarted;
            _targetChooser.SelectionEnded -= OnSelectionEnded;
            _bus.Unsubscribe<TurnStartingEvent>(OnTurnStarted);
        }

        private void OnUnitAddedToInitiative(InitiativeQueueChangedEvent ev)
        {
            if (_cameraController.SavedCommandsCount > 1)
                return;
            
            var target = _cameraController.Targets.FirstOrDefault();
            if (target == null)
                return;

            var targetsTeam = UnitTeamsUtils.GetTeam(target.GetComponent<GameObject>());
            var changedUnitTeam = UnitTeamsUtils.GetTeam(ev.ChangedUnit.Unit);

            if (targetsTeam == changedUnitTeam)
            {
                var newTargetsGroup = _cameraController.Targets.ToList();
                newTargetsGroup.Add(ev.ChangedUnit.Unit.GameObject.transform);
                _cameraController.SetTargets(_cameraController.Follow.ToArray(), newTargetsGroup, false);
            }
            else
            {
                var newFollowGroup = _cameraController.Follow.ToList();
                newFollowGroup.Add(ev.ChangedUnit.Unit.GameObject.transform);
                _cameraController.SetTargets(newFollowGroup, _cameraController.Targets.ToArray(), false);
            }
        }

        private void OnTurnStarted(TurnStartingEvent ev)
        {
            while (_cameraController.SavedCommandsCount > 1)
                _cameraController.ReturnToBack();

            var follow = ev.UnitData.Unit.GameObject.transform;
            var followTeam = UnitTeamsUtils.GetTeam(ev.UnitData.Unit);
            var targets = UnityEngine.Pool.ListPool<IBattleUnit>.Get();
            targets.Clear();

            foreach (var data in _queue.Queue)
            {
                var team = UnitTeamsUtils.GetTeam(data.Unit);
                if (team != followTeam)
                    targets.Add(data.Unit);
            }
            
            _cameraController.ReturnToBack();
            _cameraController.SetTargets(
                follow,
                targets.Select(x => x.GameObject.transform),
                true,
                ZoomType.Close);
            
            UnityEngine.Pool.ListPool<IBattleUnit>.Release(targets);
        }

        private void OnSelectionStarted(ChooseTargetCommand command)
        {
            var targets = _queue.Queue
                .Where(x =>
                    command.Filter.GetMaskForPlayer().HasTeam(UnitTeamsUtils.GetTeam(x.Unit)))
                .Select(x => x.Unit.GameObject.transform);
            
            _cameraController.SetTargets(
                Array.Empty<Transform>(), targets, true, ZoomType.Far);
        }

        private void OnSelectionEnded()
        {
            _cameraController.ReturnToBack();
        }
    }
}