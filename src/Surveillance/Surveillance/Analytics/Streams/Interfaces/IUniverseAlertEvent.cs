using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Analytics.Streams.Interfaces
{
    public interface IUniverseAlertEvent
    {
        Domain.Scheduling.Rules Rule { get; }
        bool IsFlushEvent { get; set; }
        bool IsRemoveEvent { get; set; }
        object UnderlyingAlert { get; }
        ISystemProcessOperationRunRuleContext Context { get; }
    }
}