using System;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    public interface IUniversePercentageCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateTimeLogger(DomainV2.Scheduling.ScheduledExecution execution);
        void InitiateEventLogger(IUniverse universe);
    }
}
