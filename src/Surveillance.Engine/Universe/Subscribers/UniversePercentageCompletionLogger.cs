namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    /// <summary>
    /// The universe percentage completion logger.
    /// </summary>
    public class UniversePercentageCompletionLogger : IUniversePercentageCompletionLogger
    {
        /// <summary>
        /// The percentage of events logger.
        /// </summary>
        private readonly IUniversePercentageOfEventCompletionLogger percentageOfEventsLogger;

        /// <summary>
        /// The percent of time logger.
        /// </summary>
        private readonly IUniversePercentageOfTimeCompletionLogger percentOfTimeLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversePercentageCompletionLogger"/> class.
        /// </summary>
        /// <param name="percentageOfEventsLogger">
        /// The percentage of events logger.
        /// </param>
        /// <param name="percentOfTimeLogger">
        /// The percent of time logger.
        /// </param>
        public UniversePercentageCompletionLogger(
            IUniversePercentageOfEventCompletionLogger percentageOfEventsLogger,
            IUniversePercentageOfTimeCompletionLogger percentOfTimeLogger)
        {
            this.percentageOfEventsLogger = percentageOfEventsLogger
                                             ?? throw new ArgumentNullException(nameof(percentageOfEventsLogger));
            this.percentOfTimeLogger =
                percentOfTimeLogger ?? throw new ArgumentNullException(nameof(percentOfTimeLogger));
        }

        /// <summary>
        /// The initiate event logger.
        /// </summary>
        /// <param name="universe">
        /// The universe.
        /// </param>
        public void InitiateEventLogger(IUniverse universe)
        {
            this.percentageOfEventsLogger?.InitiateEventLogger(universe);
        }

        /// <summary>
        /// The initiate time logger.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        public void InitiateTimeLogger(ScheduledExecution execution)
        {
            this.percentOfTimeLogger?.InitiateTimeLogger(execution);
        }

        /// <summary>
        /// The on completed.
        /// </summary>
        public void OnCompleted()
        {
            this.percentageOfEventsLogger.OnCompleted();
            this.percentOfTimeLogger.OnCompleted();
        }

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnError(Exception error)
        {
            this.percentageOfEventsLogger.OnError(error);
            this.percentOfTimeLogger.OnError(error);
        }

        /// <summary>
        /// The on next.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            this.percentageOfEventsLogger.OnNext(value);
            this.percentOfTimeLogger.OnNext(value);
        }
    }
}