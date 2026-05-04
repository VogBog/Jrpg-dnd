using System;
using System.Diagnostics.Contracts;
using Game.Scripts.Battle.UnitsTeam;

namespace Game.Scripts.Battle.TargetChooser.Data
{
    [Serializable]
    public struct TargetFilter
    {
        public bool Allies;
        public bool Enemies;

        [Pure]
        public UnitTeamsMask GetMaskForPlayer()
        {
            var mask = UnitTeamsMask.None();
            if (Allies)
                mask.AddTeam(UnitTeams.Ally);
            if (Enemies)
                mask.AddTeam(UnitTeams.Enemy);
            return mask;
        }

        [Pure]
        public UnitTeamsMask GetMaskForEnemy()
        {
            var mask = UnitTeamsMask.None();
            if (Allies)
                mask.AddTeam(UnitTeams.Enemy);
            if (Enemies)
                mask.AddTeam(UnitTeams.Ally);
            return mask;
        }

        public static TargetFilter All() => new TargetFilter
        {
            Allies = true,
            Enemies = true,
        };
    }
}