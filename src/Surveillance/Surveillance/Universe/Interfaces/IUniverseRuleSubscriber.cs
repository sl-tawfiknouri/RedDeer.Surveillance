using System.Threading.Tasks;
using Domain.Scheduling;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseRuleSubscriber
    {
        Task SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationContext opCtx);
    }
}