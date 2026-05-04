using System;
using Game.Scripts.Battle;
using Game.Scripts.Battle.UnitsTeam;
using Zenject;

namespace Game.Helpers.ZenjectHelpers
{
    public static class ZenjectExtensions
    {
        public static CopyNonLazyBinder WhenTeam(
            this ConcreteIdArgConditionCopyNonLazyBinder binder,
            Predicate<UnitTeams> filter)
        {
            return binder.When(ctx =>
                filter.Invoke(UnitTeamsUtils.GetTeam(ctx.ObjectInstance)));
        }
        
        public static CopyNonLazyBinder WhenAIControls(
            this ConcreteIdArgConditionCopyNonLazyBinder binder)
        {
            return binder.When(ctx =>
                UnitTeamsUtils.IsUnderAIControl(ctx.Container.TryResolve<IBattleUnit>()));
        }

        public static CopyNonLazyBinder WhenNotAI(
            this ConcreteIdArgConditionCopyNonLazyBinder binder)
        {
            return binder.When(ctx =>
                !UnitTeamsUtils.IsUnderAIControl(ctx.Container.TryResolve<IBattleUnit>()));
        }
    }
}