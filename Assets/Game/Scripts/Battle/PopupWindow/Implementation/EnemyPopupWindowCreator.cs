using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.PopupWindow.Data;
using Game.Scripts.Battle.PopupWindow.Interfaces;
using UnityEngine;

namespace Game.Scripts.Battle.PopupWindow.Implementation
{
    public class EnemyPopupWindowCreator : IPopupWindowCreator
    {
        public async UniTask<(bool, PopupWindowOption)> OpenAsync(IList<PopupWindowOption> options, Sprite icon, string question, CancellationToken ct)
        {
            if (options == null || options.Count == 0)
                return (false, default);

            int randIndex = Random.Range(0, options.Count + 1);
            if (randIndex == options.Count)
                return (false, default);
            
            return (true, options[randIndex]);
        }
    }
}