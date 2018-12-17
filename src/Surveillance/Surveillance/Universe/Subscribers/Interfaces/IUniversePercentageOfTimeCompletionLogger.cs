using System;
using DomainV2.Scheduling;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Subscribers.Interfaces
{
    public interface IUniversePercentageOfTimeCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateTimeLogger(ScheduledExecution execution);
    }
}
