namespace Surveillance.Engine.Rules.Tests.Universe.Subscriber
{
    using System;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Universe;
    using Surveillance.Engine.Rules.Universe.Subscribers;

    [TestFixture]
    public class UniversePercentageOfTimeCompletionLoggerTests
    {
        private ILogger<UniversePercentageOfTimeCompletionLogger> _logger;

        [Test]
        [Explicit]
        public void InitiateTimeLogger_LogsWhenExpected_3HourRun()
        {
            var timeCompletionLogger = new UniversePercentageOfTimeCompletionLogger(this._logger);
            var scheduledExecution = new ScheduledExecution
                                         {
                                             TimeSeriesInitiation = DateTimeOffset.Now,
                                             TimeSeriesTermination = DateTimeOffset.Now.AddMinutes(100)
                                         };

            timeCompletionLogger.InitiateTimeLogger(scheduledExecution);

            for (var i = 0; i < 100; i++)
            {
                var events = new UniverseEvent(
                    UniverseStateEvent.Order,
                    scheduledExecution.TimeSeriesInitiation.AddMinutes(i).DateTime,
                    new object());

                timeCompletionLogger.OnNext(events);
            }
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger<UniversePercentageOfTimeCompletionLogger>>();
        }
    }
}