using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Streams
{
    public class UniverseAlertEvent : IUniverseAlertEvent
    {
        public UniverseAlertEvent(
            Domain.Surveillance.Scheduling.Rules rule, 
            object underlyingAlert,
            ISystemProcessOperationRunRuleContext context,
            bool isFlushEvent = false,
            bool isDeleteEvent = false)
        {
            Rule = rule;
            UnderlyingAlert = underlyingAlert;
            Context = context;
            IsFlushEvent = isFlushEvent;
            IsDeleteEvent = isDeleteEvent;
        }

        public Domain.Surveillance.Scheduling.Rules Rule { get; }
        public bool IsFlushEvent { get; set; }
        public bool IsDeleteEvent { get; set; }
        public bool IsRemoveEvent { get; set; }
        public object UnderlyingAlert { get; }
        public ISystemProcessOperationRunRuleContext Context { get; }
    }
}
