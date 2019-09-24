namespace Surveillance.Engine.Rules.Tests.Factories
{
    using System;
    using System.Threading;

    using Domain.Surveillance.Streams.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Factories;

    [TestFixture]
    public class UniversePlayerFactoryTests
    {
        private ILogger<UniversePlayer> _logger;

        private IUnsubscriberFactory<IUniverseEvent> _universeEventUnsubscriberFactory;

        [Test]
        public void Build_Returns_NonNull_UniversePlayer()
        {
            var factory = new UniversePlayerFactory(this._universeEventUnsubscriberFactory, this._logger);

            var universePlayer = factory.Build(new CancellationToken());

            Assert.IsInstanceOf<UniversePlayer>(universePlayer);
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniversePlayerFactory(this._universeEventUnsubscriberFactory, null));
        }

        [Test]
        public void Constructor_UnsubscriberFactory_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniversePlayerFactory(null, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._universeEventUnsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
            this._logger = new NullLogger<UniversePlayer>();
        }
    }
}