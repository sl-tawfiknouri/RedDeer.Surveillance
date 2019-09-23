namespace Surveillance.Engine.Rules.Tests.Analytics.Streams.Factory
{
    using System;

    using Domain.Surveillance.Streams.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Factory;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

    [TestFixture]
    public class UniverseAlertStreamFactoryTests
    {
        private ILogger<UniverseAlertStream> _logger;

        private IUnsubscriberFactory<IUniverseAlertEvent> _unsubscriberFactory;

        [Test]
        public void Build_Returns_Non_Null_Stream()
        {
            var factory = new UniverseAlertStreamFactory(this._unsubscriberFactory, this._logger);

            var alertStream = factory.Build();

            Assert.IsNotNull(alertStream);
        }

        [Test]
        public void Constructor_Considers_Null_Logger_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseAlertStreamFactory(this._unsubscriberFactory, null));
        }

        [Test]
        public void Constructor_Considers_Null_Unsubscriber_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseAlertStreamFactory(null, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._unsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseAlertEvent>>();
            this._logger = new NullLogger<UniverseAlertStream>();
        }
    }
}