namespace Surveillance.Engine.Rules.Analytics.Streams
{
    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

    public class UniverseAlertEvent : IUniverseAlertEvent
    {
        public UniverseAlertEvent(
            Rules rule,
            object underlyingAlert,
            ISystemProcessOperationRunRuleContext context,
            bool isFlushEvent = false,
            bool isDeleteEvent = false)
        {
            this.Rule = rule;
            this.UnderlyingAlert = underlyingAlert;
            this.Context = context;
            this.IsFlushEvent = isFlushEvent;
            this.IsDeleteEvent = isDeleteEvent;
        }

        public ISystemProcessOperationRunRuleContext Context { get; }

        public bool IsDeleteEvent { get; set; }

        public bool IsFlushEvent { get; set; }

        public bool IsRemoveEvent { get; set; }

        public Rules Rule { get; }

        public object UnderlyingAlert { get; }
    }
}