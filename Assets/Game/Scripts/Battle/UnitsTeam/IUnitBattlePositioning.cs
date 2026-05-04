using UnityEngine;

namespace Game.Scripts.Battle.UnitsTeam
{
    public interface IUnitBattlePositioning
    {
        Vector3? GetPositionForUnit(ITeamUnit teamUnit);
    }
}