namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    public class UniversePercentageCompletionLogger : IUniversePercentageCompletionLogger
    {
        private readonly IUniversePercentageOfEventCompletionLogger _percentageOfEventsLogger;

        private readonly IUniversePercentageOfTimeCompletionLogger _percentOfTimeLogger;

        public UniversePercentageCompletionLogger(
            IUniversePercentageOfEventCompletionLogger percentageOfEventsLogger,
            IUniversePercentageOfTimeCompletionLogger percentOfTimeLogger)
        {
            this._percentageOfEventsLogger = percentageOfEventsLogger
                                             ?? throw new ArgumentNullException(nameof(percentageOfEventsLogger));
            this._percentOfTimeLogger =
                percentOfTimeLogger ?? throw new ArgumentNullException(nameof(percentOfTimeLogger));
        }

        public void InitiateEventLogger(IUniverse universe)
        {
            this._percentageOfEventsLogger?.InitiateEventLogger(universe);
        }

        public void InitiateTimeLogger(ScheduledExecution execution)
        {
            this._percentOfTimeLogger?.InitiateTimeLogger(execution);
        }

        public void OnCompleted()
        {
            this._percentageOfEventsLogger.OnCompleted();
            this._percentOfTimeLogger.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this._percentageOfEventsLogger.OnError(error);
            this._percentOfTimeLogger.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            this._percentageOfEventsLogger.OnNext(value);
            this._percentOfTimeLogger.OnNext(value);
        }
    }
}