namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The UniversePercentageCompletionLogger interface.
    /// </summary>
    public interface IUniversePercentageCompletionLogger : IObserver<IUniverseEvent>
    {
        /// <summary>
        /// The initiate event logger.
        /// </summary>
        /// <param name="universe">
        /// The universe.
        /// </param>
        void InitiateEventLogger(IUniverse universe);

        /// <summary>
        /// The initiate time logger.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        void InitiateTimeLogger(ScheduledExecution execution);
    }
}