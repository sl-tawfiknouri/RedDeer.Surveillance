namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;

    public interface IUniverseRuleSubscriber
    {
        Task<IReadOnlyCollection<string>> SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            ISystemProcessOperationContext opCtx,
            RuleParameterDto ruleParameters);
    }
}