namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IUniversePercentageOfTimeCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateTimeLogger(ScheduledExecution execution);
    }
}