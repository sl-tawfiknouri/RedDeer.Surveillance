using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Streams.Interfaces
{
    public interface IUniverseAlertEvent
    {
        Domain.Scheduling.Rules Rule { get; }
        bool IsFlushEvent { get; set; }
        bool IsRemoveEvent { get; set; }
        bool IsDeleteEvent { get; set; }
        object UnderlyingAlert { get; }
        ISystemProcessOperationRunRuleContext Context { get; }
    }
}