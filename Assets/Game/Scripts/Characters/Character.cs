using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle;
using Game.Scripts.Battle.DamageDealing.Health.Events;
using Game.Scripts.Battle.UnitsTeam;
using Game.Scripts.Events.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Characters
{
    public class Character : MonoBehaviour, IBattleUnit, ITeamUnit
    {
        private IEventBus _bus;
        
        [SerializeField] private UnitTeams _team;
        
        public Sprite Icon { get; set; }
        public bool IsMyTurn { get; private set; } = false;
        public string Name => "Character " + _team;
        
        public DiContainer UnitContainer { get; private set; }

        public GameObject GameObject => gameObject;

        [Inject]
        private void Construct(DiContainer container, IEventBus bus)
        {
            UnitContainer = container;
            _bus = bus;
            bus.Subscribe<DiedEvent>(OnUnitDied).When(x => x.Unit == this);
        }

        public UniTask OnEndTurnAsync(CancellationToken ct)
        {
            IsMyTurn = false;
            return UniTask.CompletedTask;
        }

        public UniTask OnStartTurnAsync(CancellationToken ct)
        {
            IsMyTurn = true;
            return UniTask.CompletedTask;
        }

        public UnitTeams GetTeam() => _team;

        public void SetBattlePosition(Vector3 position)
        {
            transform.position = position;
        }

        private void OnUnitDied(DiedEvent ev)
        {
            StartCoroutine(DieRoutine());
        }

        private IEnumerator DieRoutine()
        {
            yield return null;
            
            _bus?.Unsubscribe<DiedEvent>(OnUnitDied);
            
            yield return new WaitForSeconds(1f);
            
            Destroy(gameObject);
        }
    }
}