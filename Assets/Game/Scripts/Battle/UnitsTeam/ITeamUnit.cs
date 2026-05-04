using UnityEngine;

namespace Game.Scripts.Battle.UnitsTeam
{
    public interface ITeamUnit
    {
        UnitTeams GetTeam();
        void SetBattlePosition(Vector3 position);
    }
}