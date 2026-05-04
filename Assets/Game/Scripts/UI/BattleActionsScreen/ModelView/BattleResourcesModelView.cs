using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Characters.CharacterResources.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.BattleActionsScreen.ModelView
{
    public class BattleResourcesModelView : MonoBehaviour
    {
        private IInitiativeQueue _initiativeQueue;
        private ICharacterResourcesHolder _resourcesHolder;

        private List<(ResourceModelView, ICharacterResource)> _resourcesData;

        public event Action Closed;
        public event Action<IEnumerable<ResourceModelView>> ResourcesListChanged;
        public event Action<ResourceModelView> SingleResourceChanged; 

        [Inject]
        private void Construct(IInitiativeQueue initiativeQueue)
        {
            _initiativeQueue = initiativeQueue;
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
            if (_resourcesHolder != null)
            {
                _resourcesHolder.ResourcesListChanged -= OnResourcesListChanged;
                _resourcesHolder.SingleResourceChanged -= OnSingleResourceChanged;
            }
            
            Closed?.Invoke();
            
            if (UnitTeamsUtils.IsUnderPlayerControl(data.Unit) &&
                data.Unit.GameObject.TryGetComponent(out ICharacterResourcesHolder resourcesHolder))
            {
                _resourcesHolder = resourcesHolder;
                _resourcesHolder.ResourcesListChanged += OnResourcesListChanged;
                _resourcesHolder.SingleResourceChanged += OnSingleResourceChanged;
                
                OnResourcesListChanged(_resourcesHolder.GetAll(), 0);
            }
            else
            {
                _resourcesHolder = null;
            }
        }

        private void OnResourcesListChanged(IEnumerable<ICharacterResource> resources, int count)
        {
            _resourcesData = resources
                .Select(x =>
                {
                    if (x is ICharacterMarkedResource markedResource)
                        return (new ResourceModelView(
                            x.Data, x.Count, x.MaxCount, markedResource.MarkedData, markedResource.MarkValue), x);
                    return (new ResourceModelView(x.Data, x.Count, x.MaxCount), x);
                })
                .ToList();
            ResourcesListChanged?.Invoke(_resourcesData.Select(x => x.Item1));
        }

        private void OnSingleResourceChanged(ICharacterResource resource)
        {
            var (resourceModelView, _) = _resourcesData.FirstOrDefault(x => x.Item2 == resource);
            if (resourceModelView.ViewData == null)
                return;

            resourceModelView.Count = resource.Count;
            resourceModelView.MaxCount = resource.MaxCount;
            
            SingleResourceChanged?.Invoke(resourceModelView);
        }
    }
}