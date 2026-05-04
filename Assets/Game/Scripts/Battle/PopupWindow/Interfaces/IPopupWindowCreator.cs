using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.PopupWindow.Data;
using UnityEngine;

namespace Game.Scripts.Battle.PopupWindow.Interfaces
{
    public interface IPopupWindowCreator
    {
        UniTask<(bool, PopupWindowOption)> OpenAsync(
            IList<PopupWindowOption> options,
            Sprite icon,
            string question,
            CancellationToken ct);
    }
}