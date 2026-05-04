using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Battle.CharacterActions.Implementations;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Characters.CharacterResources.DefaultResources;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.BattleActionsScreen.ModelView
{
    public class BattleActionsModelView : MonoBehaviour
    {
        private IInitiativeQueue _initiativeQueue;
        private ICharacterActionsHolder _actionsHolder;

        private readonly List<BattleActionsPage> _pages = new();
        private int _pageIndex = -1;

        public event Action Opened;
        public event Action Closed;
        public event Action<IEnumerable<ICharacterAction>> ActionsChanged;
        public event Action<IEnumerable<string>> PagesChanged;

        public event Action<ActionInfo> ActionInfoShowing;
        public event Action ActionInfoHided;
        
        [Inject]
        private void Construct(IInitiativeQueue initiativeQueue)
        {
            _initiativeQueue = initiativeQueue;
        }

        public void SelectPage(int pageIndex)
        {
            if (_pageIndex == pageIndex || _actionsHolder == null || _pages.Count == 0)
                return;
            
            _pageIndex = pageIndex;
            ActionsChanged?.Invoke(_pages[pageIndex].FilterActions(_actionsHolder.GetAllActions()));
        }

        public void ShowActionInfo(ICharacterAction action)
        {
            var data = action as ICharacterActionData;
            var usedMarkedResources = action
                .UseMarkedResources.Select(x => (x.Data, x.MarkValue));
            var info = new ActionInfo(data, action.UseResources, usedMarkedResources);
            ActionInfoShowing?.Invoke(info);
        }

        public void HideActionInfo()
        {
            ActionInfoHided?.Invoke();
        }

        private void OnEnable()
        {
            _initiativeQueue.NextTurnStarted += OnNextTurnStarted;
        }

        private void OnDisable()
        {
            _initiativeQueue.NextTurnStarted -= OnNextTurnStarted;
        }

        private void OnNextTurnStarted(InitiativeUnitData data)
        {
            _actionsHolder = null;
            if (UnitTeamsUtils.IsUnderPlayerControl(data.Unit))
            {
                Opened?.Invoke();
                if (data.Unit.GameObject.TryGetComponent(out ICharacterActionsHolder actionsHolder))
                {
                    _actionsHolder = actionsHolder;
                    StartCoroutine(CreatePagesRoutine(actionsHolder));
                }
            }
            else
            {
                Closed?.Invoke();
            }
        }

        private IEnumerator CreatePagesRoutine(ICharacterActionsHolder actionsHolder)
        {
            _pages.Clear();
            _pageIndex = -1;
            PagesChanged?.Invoke(Array.Empty<string>());
            ActionsChanged?.Invoke(Array.Empty<ICharacterAction>());

            foreach (var page in GetAllPages())
            {
                var items = page.FilterActions(actionsHolder.GetAllActions());
                if (items.Any())
                    _pages.Add(page);
                yield return null;
            }

            if (_pages.Count == 0)
                yield break;
            
            PagesChanged?.Invoke(_pages.Select(x => x.Name));
            ActionsChanged?.Invoke(_pages[0].FilterActions(actionsHolder.GetAllActions()));
        }

        private IEnumerable<BattleActionsPage> GetAllPages()
        {
            yield return new BattleActionsPage("All", x => true);
            
            yield return new BattleActionsPage("Actions",
                x => x is IActiveAction and not IPassiveEffect and not BaseSpell);

            yield return new BattleActionsPage("Spells",
                x => x is BaseSpell and not IPassiveEffect);

            yield return new BattleActionsPage("Passives",
                x => x is IPassiveEffect);
        }
    }
}