using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.PopupWindow.Data;
using Game.Scripts.Battle.PopupWindow.Interfaces;
using Game.Scripts.UI.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Battle.PopupWindow.Implementation
{
    public class PlayerPopupWindowCreator : MonoBehaviour, IPopupWindowCreator
    {
        [SerializeField] private GameObject _window;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private UIObjectsPool<Button> _pool;

        private bool _selecting = false;
        private int _chooseIndex = -1;

        public async UniTask<(bool, PopupWindowOption)> OpenAsync(IList<PopupWindowOption> options, Sprite icon, string question, CancellationToken ct)
        {
            if (_selecting)
                return (false, default);
            
            if (options == null || options.Count == 0)
                return (false, default);

            _selecting = true;
            _chooseIndex = -1;
            _icon.sprite = icon;
            _questionText.text = question;
            
            int i = 0;
            foreach (var option in options)
            {
                await CreateOption(option, i, ct);
                ++i;
            }

            await CreateOption(new PopupWindowOption("No"), i, ct);
            
            _window.SetActive(true);

            await UniTask.WaitWhile(() => _selecting, cancellationToken: ct);
            
            foreach (var button in _pool.UsingItems) 
                button.onClick.RemoveAllListeners();
            _pool.ReturnAll();
            _window.SetActive(false);

            if (_chooseIndex == i)
                return (false, default);
            
            return (true, options[_chooseIndex]);
        }

        private async UniTask CreateOption(PopupWindowOption option, int i, CancellationToken ct)
        {
            var viewItem = await _pool.GetAsync(ct);
            viewItem.GetComponentInChildren<TMP_Text>().text = option.Name;
            int index = i;
            viewItem.onClick.AddListener(() => OnOptionClicked(index));
            viewItem.gameObject.SetActive(true);
        }

        private void OnOptionClicked(int index)
        {
            _chooseIndex = index;
            _selecting = false;
        }
        
        private void Awake()
        {
            _pool.InitializeAsync(CancellationToken.None).Forget(); //Устал сегодня
        }

        private void OnDisable()
        {
            _pool.Dispose();
        }
    }
}