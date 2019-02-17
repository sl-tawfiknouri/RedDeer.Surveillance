﻿using System;
using Domain.Equity.Streams.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Factory;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Analytics.Streams.Factory
{
    [TestFixture]
    public class UniverseAlertStreamFactoryTests
    {
        private IUnsubscriberFactory<IUniverseAlertEvent> _unsubscriberFactory;
        private ILogger<UniverseAlertStream> _logger;

        [SetUp]
        public void Setup()
        {
            _unsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseAlertEvent>>();
            _logger = new NullLogger<UniverseAlertStream>();
        }

        [Test]
        public void Constructor_Considers_Null_Unsubscriber_To_Be_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseAlertStreamFactory(null, _logger));
        }

        [Test]
        public void Constructor_Considers_Null_Logger_To_Be_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseAlertStreamFactory(_unsubscriberFactory, null));
        }

        [Test]
        public void Build_Returns_Non_Null_Stream()
        {
            var factory = new UniverseAlertStreamFactory(_unsubscriberFactory, _logger);

            var alertStream = factory.Build();

            Assert.IsNotNull(alertStream);
        }
    }
}
