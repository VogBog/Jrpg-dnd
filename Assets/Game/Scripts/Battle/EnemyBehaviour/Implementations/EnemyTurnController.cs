using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.CharacterActions.Interfaces;
using Game.Scripts.Battle.InitiativeQueue;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Battle.EnemyBehaviour.Implementations
{
    public class EnemyTurnController : MonoBehaviour
    {
        private IInitiativeQueue _queue;
        private IAsyncOperationScope _scope;
        
        [Inject]
        private void Construct(IInitiativeQueue queue, IAsyncOperationScope scope)
        {
            _queue = queue;
            _scope = scope;
        }

        private void OnEnable()
        {
            _queue.NextTurnStarted += OnNextTurnStarted;
        }

        private void OnDisable()
        {
            _queue.NextTurnStarted -= OnNextTurnStarted;
        }

        private void OnNextTurnStarted(InitiativeUnitData data)
        {
            if (!UnitTeamsUtils.IsUnderAIControl(data.Unit))
                return;

            if (!data.Unit.GameObject.TryGetComponent(out ICharacterActionsHolder actionsHolder))
            {
                _queue.EndTurn();
            }
            
            if (_scope.TryGetToken(out var ct))
                _scope.FireAndForget(TryMakeTurnAsync(actionsHolder, data.Unit, ct));
        }

        private async UniTask TryMakeTurnAsync(ICharacterActionsHolder actions, IBattleUnit unit, CancellationToken ct)
        {
            await MakeATurnAsync(actions, unit, ct);
            _queue.EndTurn();
        }

        private async UniTask MakeATurnAsync(ICharacterActionsHolder actions, IBattleUnit unit, CancellationToken ct)
        {
            while (true)
            {
                var allActions = actions
                    .GetAllActions()
                    .OfType<IActiveAction>()
                    .Where(x => x.CanUse())
                    .ToList();

                if (allActions.Count == 0)
                    return;

                int randIndex = Random.Range(0, allActions.Count);
                await actions.TryUse(allActions[randIndex], ct);
                await UniTask.WaitForSeconds(1f, cancellationToken: ct);
            }
        }
    }
}