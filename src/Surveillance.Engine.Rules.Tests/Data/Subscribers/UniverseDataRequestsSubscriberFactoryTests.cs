namespace Surveillance.Engine.Rules.Tests.Data.Subscribers
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers;
    using Surveillance.Engine.Rules.Queues.Interfaces;

    [TestFixture]
    public class UniverseDataRequestsSubscriberFactoryTests
    {
        private ISystemProcessOperationContext _ctx;

        private ILogger<UniverseDataRequestsSubscriber> _logger;

        private IQueueDataSynchroniserRequestPublisher _publisher;

        [Test]
        public void Build_Returns_Non_Null()
        {
            var factory = new UniverseDataRequestsSubscriberFactory(this._publisher, this._logger);

            var result = factory.Build(this._ctx);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Constructor_Considers_Null_Logger_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniverseDataRequestsSubscriberFactory(this._publisher, null));
        }

        [Test]
        public void Constructor_Considers_Null_Message_Sender_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseDataRequestsSubscriberFactory(null, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._publisher = A.Fake<IQueueDataSynchroniserRequestPublisher>();
            this._logger = A.Fake<ILogger<UniverseDataRequestsSubscriber>>();
            this._ctx = A.Fake<ISystemProcessOperationContext>();
        }
    }
}