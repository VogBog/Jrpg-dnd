namespace Game.Scripts.Battle.CharacterActions.Interfaces
{
    public interface IPassiveEffect
    {
        void OnAdded(IBattleUnit owner);
        void OnRemoved();
    }
}