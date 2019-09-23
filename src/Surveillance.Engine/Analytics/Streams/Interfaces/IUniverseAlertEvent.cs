namespace Surveillance.Engine.Rules.Analytics.Streams.Interfaces
{
    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;

    public interface IUniverseAlertEvent
    {
        ISystemProcessOperationRunRuleContext Context { get; }

        bool IsDeleteEvent { get; set; }

        bool IsFlushEvent { get; set; }

        bool IsRemoveEvent { get; set; }

        Rules Rule { get; }

        object UnderlyingAlert { get; }
    }
}