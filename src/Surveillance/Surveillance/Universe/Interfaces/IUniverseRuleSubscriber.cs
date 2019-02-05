using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseRuleSubscriber
    {
        Task<IReadOnlyCollection<string>> SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ISystemProcessOperationContext opCtx,
            RuleParameterDto ruleParameters);
    }
}