using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.DamageDealing.Health.Events;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Events.Data;
using Game.Scripts.Events.Interfaces;
using Game.Scripts.Scenes;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.Finisher
{
    public class BattleFinisher : MonoBehaviour
    {
        [Inject] private IEventBus _bus;
        [Inject] private IInitiativeQueue _queue;
        [Inject] private ISceneChanger _sceneChanger;
        [Inject] private IAsyncOperationScope _scope;

        [SerializeField] private GameObject _finishPanel;
        [SerializeField] private TMP_Text _finishText;
        
        private CancellationTokenSource _cts = new();

        private void OnEnable()
        {
            _bus.Subscribe<DiedEvent>(OnUnitDied).On(EventStep.FinishGame);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<DiedEvent>(OnUnitDied);
        }

        private void OnUnitDied(DiedEvent ev)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
            
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(FinishAsync(_cts.Token, ct));
        }

        private async UniTask FinishAsync(CancellationToken ct, CancellationToken ct2)
        {
            await UniTask.WaitForSeconds(1f, cancellationToken: ct2);

            if (ct.IsCancellationRequested || ct2.IsCancellationRequested)
                return;
            
            bool hasPlayerUnits = false;
            bool hasEnemyUnits = false;

            foreach (var unit in _queue.Queue)
            {
                var team = UnitTeamsUtils.GetTeam(unit.Unit);
                if (team is UnitTeams.Ally)
                    hasPlayerUnits = true;
                else
                    hasEnemyUnits = true;
            }

            if (hasPlayerUnits && hasEnemyUnits)
                return;

            string finishText = hasPlayerUnits ? "WIN" : "FAIL";
            
            _finishText.text = finishText;
            _finishPanel.SetActive(true);

            await UniTask.WaitForSeconds(4f, cancellationToken: ct2);

            if (ct.IsCancellationRequested || ct2.IsCancellationRequested)
                return;
            
            _sceneChanger.LoadScene(0);
        }
    }
}