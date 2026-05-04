using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Battle.DamageDealing.Damage;
using Game.Scripts.Battle.DamageDealing.Health.Data;

namespace Game.Scripts.Battle.DamageDealing.Health.Interfaces
{
    public interface IHealthProcessor
    {
        int Value { get; }
        int MaxValue { get; }

        event Action<IHealthProcessor> Changed;

        IEnumerable<(DamageType, DamageReactionTypes)> GetDamageReactions();
        UniTask DealDamage(DealDamageCommand command, CancellationToken ct);
        UniTask Heal(HealCommand command, CancellationToken ct);
        void Set(int value);

        void SetDamageReaction(DamageType type, DamageReactionTypes reaction);
    }
}