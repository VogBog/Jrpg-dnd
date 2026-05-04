using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Characters.CharacterResources.Data;
using Game.Scripts.Characters.CharacterResources.Interfaces;

namespace Game.Scripts.Characters.CharacterResources.Implementations
{
    public class PlayerResourceMarkLevelChooser : IPlayerResourceMarkLevelChooser
    {
        public event Action<ChooseMarkLevelCommand> Opened;
        public event Action Closed;

        private ChooseMarkLevelCommand _command;
        private bool _choosing;
        private bool _cancelled;
        private int _level;
        
        public async UniTask<(bool, int)> ChooseMarkLevel(ChooseMarkLevelCommand command, CancellationToken ct)
        {
            if (_choosing)
                return (false, -1);

            _cancelled = false;
            _command = command;
            _choosing = true;
            Opened?.Invoke(command);
            
            await UniTask.WaitWhile(() => _choosing, cancellationToken: ct);
            
            Closed?.Invoke();
            if (_cancelled)
                return (false, -1);
            
            return (true, _level);
        }

        public void Select(int level)
        {
            bool canUse = _command.Holder.GetMarked(_command.Data, level).CanUse();
            if (!canUse)
                return;
            
            _level = level;
            _choosing = false;
        }

        public void Cancel()
        {
            _cancelled = true;
            _choosing = false;
        }
    }
}