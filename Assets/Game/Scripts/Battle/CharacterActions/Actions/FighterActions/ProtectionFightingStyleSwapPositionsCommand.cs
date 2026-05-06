using UnityEngine;

namespace Game.Scripts.Battle.CharacterActions.Actions.FighterActions
{
    public struct ProtectionFightingStyleSwapPositionsCommand
    {
        public Transform SwapWith;
        public Vector3 MyPosition;
        public Vector3 TargetPosition;

        public ProtectionFightingStyleSwapPositionsCommand(
            Transform swapWith,
            Vector3 targetPosition,
            Vector3 myPosition)
        {
            SwapWith = swapWith;
            TargetPosition = targetPosition;
            MyPosition = myPosition;
        }
    }
}