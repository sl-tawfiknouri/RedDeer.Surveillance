using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseRuleSubscriber
    {
        Task<IReadOnlyCollection<string>> SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationContext opCtx);
    }
}