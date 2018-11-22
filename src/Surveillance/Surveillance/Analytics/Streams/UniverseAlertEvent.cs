using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Analytics.Streams
{
    public class UniverseAlertEvent : IUniverseAlertEvent
    {
        public UniverseAlertEvent(
            Domain.Scheduling.Rules rule, 
            object underlyingAlert,
            ISystemProcessOperationRunRuleContext context,
            bool isFlushEvent = false)
        {
            Rule = rule;
            UnderlyingAlert = underlyingAlert;
            Context = context;
            IsFlushEvent = isFlushEvent;
        }

        public Domain.Scheduling.Rules Rule { get; }
        public bool IsFlushEvent { get; set; }
        public bool IsRemoveEvent { get; set; }
        public object UnderlyingAlert { get; }
        public ISystemProcessOperationRunRuleContext Context { get; }
    }
}
