using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Battle.DamageDealing.Health.Data;
using Game.Scripts.Scenes;
using Game.Scripts.UI.Helpers;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.CharacterAnalyze
{
    [RequireComponent(typeof(CharacterAnalyzeModelView))]
    public class CharacterAnalyzeDamageReactionsView : MonoBehaviour
    {
        [SerializeField] private UIObjectsPool<CharacterAnalyzeDamageReactionViewItem> _pool;
        
        private IAsyncOperationScope _scope;
        private CharacterAnalyzeModelView _mw;

        [Inject]
        private void Construct(IAsyncOperationScope scope)
        {
            _scope = scope;
            _mw = GetComponent<CharacterAnalyzeModelView>();
        }

        private void Awake()
        {
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(_pool.InitializeAsync(ct));
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        private void OnEnable()
        {
            _mw.UnitChanged += OnUnitChanged;
        }

        private void OnDisable()
        {
            _mw.UnitChanged -= OnUnitChanged;
        }

        private void OnUnitChanged(CharacterAnalyzeRedrawData data)
        {
             if (_scope.TryGetToken(out var ct))
                 _scope.FireAndForget(UpdateUnitAsync(data.DamageReactions, ct));
        }

        private async UniTask UpdateUnitAsync(
            IEnumerable<(DamageType, DamageReactionTypes)> reactions,
            CancellationToken ct)
        {
            _pool.ReturnAll();

            if (reactions == null)
                return;

            foreach (var (damage, reaction) in reactions)
            {
                var item = await _pool.GetAsync(ct);
                item.Icon.sprite = damage.Icon;
                item.Text.text = reaction switch
                {
                    DamageReactionTypes.Default => "OK",
                    DamageReactionTypes.Resist => "RESIST",
                    DamageReactionTypes.Immune => "IMMUNE",
                    DamageReactionTypes.Weakness => "WEAK",
                    _ => "??"
                };
                item.InfoText = $"{damage.Name} - {reaction.ToString()}";
                item.Icon.preserveAspect = true;
                
                item.gameObject.SetActive(true);
            }
        }
    }
}