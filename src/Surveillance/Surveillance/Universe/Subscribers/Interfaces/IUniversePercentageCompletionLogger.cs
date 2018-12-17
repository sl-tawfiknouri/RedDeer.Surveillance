using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Subscribers.Interfaces
{
    public interface IUniversePercentageCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateTimeLogger(DomainV2.Scheduling.ScheduledExecution execution);
        void InitiateEventLogger(IUniverse universe);
    }
}
