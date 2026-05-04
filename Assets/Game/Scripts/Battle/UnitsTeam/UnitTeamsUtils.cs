using JetBrains.Annotations;
using UnityEngine;

namespace Game.Scripts.Battle.UnitsTeam
{
    public static class UnitTeamsUtils
    {
        [CanBeNull]
        public static ITeamUnit GetTeamUnit<T>(T from)
        {
            if (from is ITeamUnit teamUnit)
                return teamUnit;

            if (from is MonoBehaviour monoBehaviour)
                return monoBehaviour.GetComponent<ITeamUnit>();
            
            if (from is GameObject go)
                return go.GetComponent<ITeamUnit>();

            return null;
        }

        public static bool TryGetTeamUnit<T>(T from, out ITeamUnit teamUnit)
        {
            teamUnit = GetTeamUnit(from);
            return teamUnit != null;
        }

        public static UnitTeams GetTeam<T>(T from)
        {
            var teamUnit = GetTeamUnit(from);
            return teamUnit?.GetTeam() ?? UnitTeams.Enemy;
        }

        public static bool IsUnderPlayerControl<T>(T from)
        {
            return GetTeam(from) is UnitTeams.Ally or UnitTeams.AllySummon;
        }

        public static bool IsUnderAIControl<T>(T from)
        {
            if (from == null)
                return false;
            
            if (!TryGetTeamUnit(from, out var unit))
                return false;

            return unit.GetTeam() is UnitTeams.Enemy;
        }
    }
}