using System;
using System.Collections.Generic;
using Game.Scripts.Battle.InitiativeQueue;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.UnitsTeam
{
    public class UnitBattlePositioning : MonoBehaviour, IUnitBattlePositioning
    {
        [SerializeField] private TeamPositions[] _positions;
        
        private IInitiativeQueue _initiativeQueue;
        private readonly Dictionary<ITeamUnit, Vector3> _usedPositions = new(8);

        [Inject]
        private void Construct(IInitiativeQueue initiativeQueue)
        {
            _initiativeQueue = initiativeQueue;
        }
        
        public Vector3? GetFreePositionForUnit(ITeamUnit teamUnit)
        {
            var team = teamUnit.GetTeam();
            var teamIndex = Array.FindIndex(_positions, x => x.Team == team);
            if (teamIndex == -1)
                throw new Exception(
                    $"UnitBattlePositioning.GetPositionForObject: Cannot get positions for team {team}");

            var pointIndex = Array.FindIndex(_positions[teamIndex].Points, x => x.Target == null);
            if (pointIndex == -1)
            {
                Debug.LogError(
                    $"UnitBattlePositioning.GetPositionForObject: Cannot get free position for team {team}. " +
                    "All positions are using.");
                return null;
            }

            _positions[teamIndex].Points[pointIndex].Target = teamUnit;
            return _positions[teamIndex].Points[pointIndex].Transform.position;
        }

        public Vector3? GetUsedPositionForUnit(ITeamUnit teamUnit)
        {
            if (_usedPositions.TryGetValue(teamUnit, out var position))
                return position;
            return null;
        }

        private void OnEnable()
        {
            _initiativeQueue.UnitAdded += OnUnitAdded;
        }

        private void OnDisable()
        {
            _initiativeQueue.UnitAdded -= OnUnitAdded;
        }

        private void OnUnitAdded(InitiativeQueueChangedEvent ev)
        {
            if (ev.ChangedUnit.Unit is MonoBehaviour monoBehaviour &&
                monoBehaviour.TryGetComponent(out ITeamUnit teamUnit))
            {
                SetPositionForUnit(teamUnit);
            }
            else if (ev.ChangedUnit.Unit is ITeamUnit teamUnit2)
            {
                SetPositionForUnit(teamUnit2);
            }
        }

        private void SetPositionForUnit(ITeamUnit unit)
        {
            var pos = GetFreePositionForUnit(unit);
            if (pos != null)
            {
                unit.SetBattlePosition(pos.Value);
                _usedPositions.Add(unit, pos.Value);
            }
        }

        
        
        
        
        [Serializable]
        public struct TeamPositions
        {
            public UnitTeams Team;
            public TeamPoint[] Points;
        }

        [Serializable]
        public struct TeamPoint
        {
            public Transform Transform;
            [HideInInspector] public ITeamUnit Target;
        }
    }
}