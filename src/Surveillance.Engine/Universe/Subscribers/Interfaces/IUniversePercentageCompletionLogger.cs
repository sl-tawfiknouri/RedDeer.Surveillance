namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IUniversePercentageCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateEventLogger(IUniverse universe);

        void InitiateTimeLogger(ScheduledExecution execution);
    }
}