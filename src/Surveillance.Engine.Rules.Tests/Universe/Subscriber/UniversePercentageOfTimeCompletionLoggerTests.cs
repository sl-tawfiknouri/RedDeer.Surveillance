using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Surveillance.Tests.Universe.Subscriber
{
    [TestFixture]
    public class UniversePercentageOfTimeCompletionLoggerTests
    {
        private ILogger<UniversePercentageOfTimeCompletionLogger> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<UniversePercentageOfTimeCompletionLogger>>();
        }
        
        [Test]
        [Explicit]
        public void InitiateTimeLogger_LogsWhenExpected_3HourRun()
        {
            var timeCompletionLogger = new UniversePercentageOfTimeCompletionLogger(_logger);
            var scheduledExecution = new DomainV2.Scheduling.ScheduledExecution
            {
                TimeSeriesInitiation = DateTimeOffset.Now,
                TimeSeriesTermination = DateTimeOffset.Now.AddMinutes(100)
            };

            timeCompletionLogger.InitiateTimeLogger(scheduledExecution);

            for (var i = 0; i < 100; i++)
            {
                var events =
                    new UniverseEvent(
                        UniverseStateEvent.Order,
                        scheduledExecution.TimeSeriesInitiation.AddMinutes(i).DateTime,
                        new object());

                timeCompletionLogger.OnNext(events);
            }
        }
    }
}
