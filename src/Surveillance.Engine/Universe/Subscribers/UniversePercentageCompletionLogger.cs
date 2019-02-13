using System;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    public class UniversePercentageCompletionLogger : IUniversePercentageCompletionLogger
    {
        private readonly IUniversePercentageOfEventCompletionLogger _percentageOfEventsLogger;
        private readonly IUniversePercentageOfTimeCompletionLogger _percentOfTimeLogger;

        public UniversePercentageCompletionLogger(
            IUniversePercentageOfEventCompletionLogger percentageOfEventsLogger,
            IUniversePercentageOfTimeCompletionLogger percentOfTimeLogger)
        {
            _percentageOfEventsLogger = percentageOfEventsLogger ?? throw new ArgumentNullException(nameof(percentageOfEventsLogger));
            _percentOfTimeLogger = percentOfTimeLogger ?? throw new ArgumentNullException(nameof(percentOfTimeLogger));
        }

        public void InitiateTimeLogger(DomainV2.Scheduling.ScheduledExecution execution)
        {
            _percentOfTimeLogger?.InitiateTimeLogger(execution);
        }

        public void InitiateEventLogger(IUniverse universe)
        {
            _percentageOfEventsLogger?.InitiateEventLogger(universe);
        }

        public void OnCompleted()
        {
            _percentageOfEventsLogger.OnCompleted();
            _percentOfTimeLogger.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _percentageOfEventsLogger.OnError(error);
            _percentOfTimeLogger.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            _percentageOfEventsLogger.OnNext(value);
            _percentOfTimeLogger.OnNext(value);
        }
    }
}
