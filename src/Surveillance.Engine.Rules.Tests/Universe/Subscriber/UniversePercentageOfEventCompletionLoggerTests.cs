﻿namespace Surveillance.Engine.Rules.Tests.Universe.Subscriber
{
    using System;
    using System.Collections.Generic;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Data.Universe;
    using Surveillance.Engine.Rules.Universe.Subscribers;

    [TestFixture]
    public class UniversePercentageOfEventCompletionLoggerTests
    {
        private ILogger<UniversePercentageOfEventCompletionLogger> _logger;

        [Test]
        [Explicit]
        public void InitiateLogging_WithEvents_LogsAsTheyOccur()
        {
            var event1 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, new object());
            var event2 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(1), new object());
            var event3 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(2), new object());
            var event4 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(3), new object());
            var event5 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(4), new object());
            var event6 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(5), new object());
            var event7 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(6), new object());
            var event8 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(7), new object());
            var event9 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(8), new object());
            var event10 = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow.AddMinutes(9), new object());

            var universeEvents = new List<UniverseEvent>
                                     {
                                         event1,
                                         event2,
                                         event3,
                                         event4,
                                         event5,
                                         event6,
                                         event7,
                                         event8,
                                         event9,
                                         event10
                                     };

            var loggerUniverse = new Universe(universeEvents);

            var logger = new UniversePercentageOfEventCompletionLogger(this._logger);
            logger.InitiateEventLogger(loggerUniverse);

            foreach (var item in universeEvents)
                logger.OnNext(item);
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger<UniversePercentageOfEventCompletionLogger>>();
        }
    }
}