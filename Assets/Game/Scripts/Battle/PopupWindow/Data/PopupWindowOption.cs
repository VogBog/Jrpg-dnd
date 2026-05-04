using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Battle.PopupWindow.Data
{
    public struct PopupWindowOption
    {
        public int Index;
        public string Name;
        public Func<PopupWindowOption, CancellationToken, UniTask> Action;

        public PopupWindowOption(string name, Func<PopupWindowOption, CancellationToken, UniTask> action = null)
        {
            Index = -1;
            Name = name;
            Action = action;
        }
    }
}