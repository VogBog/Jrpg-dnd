using UnityEngine;

namespace Game.Scripts.UI.CharacterAnalyze
{
    [RequireComponent(typeof(CharacterAnalyzeModelView), typeof(CharacterAnalyzeView))]
    public class CharacterAnalyzeMouseController : MonoBehaviour
    {
        private CharacterAnalyzeModelView _mw;
        private CharacterAnalyzeView _view;
        
        private void Awake()
        {
            _mw = GetComponent<CharacterAnalyzeModelView>();
            _view = GetComponent<CharacterAnalyzeView>();
        }

        private void OnEnable()
        {
            _view.QueueItemCreated += OnQueueItemCreated;
        }

        private void OnDisable()
        {
            _view.QueueItemCreated -= OnQueueItemCreated;
        }

        private void OnQueueItemCreated(CharacterAnalyzeQueueItem item)
        {
            item.Button.onClick.AddListener(() => _mw.Select(item.Target));
        }
    }
}