using UnityEngine;

namespace Game.Scripts.Battle.UnitsTeam
{
    public interface IUnitBattlePositioning
    {
        Vector3? GetFreePositionForUnit(ITeamUnit teamUnit);
        Vector3? GetUsedPositionForUnit(ITeamUnit teamUnit);
    }
}