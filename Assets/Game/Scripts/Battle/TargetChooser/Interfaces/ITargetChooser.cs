using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.TargetChooser.Data;

namespace Game.Scripts.Battle.TargetChooser.Interfaces
{
    public interface ITargetChooser
    {
        UniTask<List<IBattleUnit>> Choose(ChooseTargetCommand command, CancellationToken ct);
    }
}