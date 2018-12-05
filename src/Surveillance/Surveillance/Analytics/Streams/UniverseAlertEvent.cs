using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Analytics.Streams
{
    public class UniverseAlertEvent : IUniverseAlertEvent
    {
        public UniverseAlertEvent(
            DomainV2.Scheduling.Rules rule, 
            object underlyingAlert,
            ISystemProcessOperationRunRuleContext context,
            bool isFlushEvent = false)
        {
            Rule = rule;
            UnderlyingAlert = underlyingAlert;
            Context = context;
            IsFlushEvent = isFlushEvent;
        }

        public DomainV2.Scheduling.Rules Rule { get; }
        public bool IsFlushEvent { get; set; }
        public bool IsRemoveEvent { get; set; }
        public object UnderlyingAlert { get; }
        public ISystemProcessOperationRunRuleContext Context { get; }
    }
}
