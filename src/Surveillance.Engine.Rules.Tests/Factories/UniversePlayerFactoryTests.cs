using System;
using DomainV2.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class UniversePlayerFactoryTests
    {
        private IUnsubscriberFactory<IUniverseEvent> _universeEventUnsubscriberFactory;
        private ILogger<UniversePlayer> _logger;

        [SetUp]
        public void Setup()
        {
            _universeEventUnsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
            _logger = new NullLogger<UniversePlayer>();
        }

        [Test]
        public void Constructor_UnsubscriberFactory_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniversePlayerFactory(null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniversePlayerFactory(_universeEventUnsubscriberFactory, null));
        }

        [Test]
        public void Build_Returns_NonNull_UniversePlayer()
        {
            var factory = new UniversePlayerFactory(_universeEventUnsubscriberFactory, _logger);

            var universePlayer = factory.Build();

            Assert.IsInstanceOf<UniversePlayer>(universePlayer);
        }
    }
}
