using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Characters.CharacterResources.Data;

namespace Game.Scripts.Characters.CharacterResources.Interfaces
{
    public interface ICharacterResourceMarkLevelChooser
    {
        UniTask<(bool, int)> ChooseMarkLevel(
            ChooseMarkLevelCommand command,
            CancellationToken ct);
            
        void Select(int level);
        void Cancel();
    }
}