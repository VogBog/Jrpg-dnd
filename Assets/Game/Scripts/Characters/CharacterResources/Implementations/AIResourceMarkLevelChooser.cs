using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Characters.CharacterResources.Data;
using Game.Scripts.Characters.CharacterResources.Interfaces;

namespace Game.Scripts.Characters.CharacterResources.Implementations
{
    public class AIResourceMarkLevelChooser : ICharacterResourceMarkLevelChooser
    {
        public async UniTask<(bool, int)> ChooseMarkLevel(ChooseMarkLevelCommand command, CancellationToken ct)
        {
            var resource = command.Holder.GetMarked(command.Data, command.MinLevel, true);
            if (!resource.CanUse())
                return (false, -1);

            return (true, resource.MarkValue);
        }

        public void Select(int level)
        {
            
        }

        public void Cancel()
        {
            
        }
    }
}