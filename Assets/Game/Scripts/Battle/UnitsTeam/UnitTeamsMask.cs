using JetBrains.Annotations;
using UnityEngine;

namespace Game.Scripts.Battle.UnitsTeam
{
    public struct UnitTeamsMask
    {
        private int _mask;

        public UnitTeamsMask AddTeam(UnitTeams team)
        {
            int value = (int)Mathf.Pow(2, (int)team);
            _mask |= value;
            return this;
        }

        public UnitTeamsMask RemoveTeam(UnitTeams team)
        {
            int value = (int)Mathf.Pow(2, (int)team);
            _mask &= ~value;
            return this;
        }

        public UnitTeamsMask AddTeams(params UnitTeams[] teams)
        {
            foreach (var team in teams)
                AddTeam(team);
            return this;
        }

        [Pure]
        public bool HasTeam(UnitTeams team)
        {
            int value = (int)Mathf.Pow(2, (int)team);
            return (_mask & value) != 0;
        }

        public UnitTeamsMask SetAll()
        {
            _mask = 0b1111_1111_1111_1111;
            return this;
        }

        public UnitTeamsMask Clear()
        {
            _mask = 0;
            return this;
        }

        public static UnitTeamsMask All()
        {
            return new UnitTeamsMask().SetAll();
        }

        public static UnitTeamsMask None()
        {
            return new UnitTeamsMask();
        }
    }
}