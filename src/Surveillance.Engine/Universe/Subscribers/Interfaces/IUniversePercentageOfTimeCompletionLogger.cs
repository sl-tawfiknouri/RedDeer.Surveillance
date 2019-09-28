namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The UniversePercentageOfTimeCompletionLogger interface.
    /// </summary>
    public interface IUniversePercentageOfTimeCompletionLogger : IObserver<IUniverseEvent>
    {
        /// <summary>
        /// The initiate time logger.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        void InitiateTimeLogger(ScheduledExecution execution);
    }
}