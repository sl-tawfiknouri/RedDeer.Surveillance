using System;
using Domain.Surveillance.Scheduling;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    public interface IUniversePercentageOfTimeCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateTimeLogger(ScheduledExecution execution);
    }
}
