using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Data.Subscribers
{
    [TestFixture]
    public class UniverseDataRequestsSubscriberFactoryTests
    {
        private IQueueDataSynchroniserRequestPublisher _publisher;
        private ILogger<UniverseDataRequestsSubscriber> _logger;
        private ISystemProcessOperationContext _ctx;

        [SetUp]
        public void Setup()
        {
            _publisher = A.Fake<IQueueDataSynchroniserRequestPublisher>();
            _logger = A.Fake<ILogger<UniverseDataRequestsSubscriber>>();
            _ctx = A.Fake<ISystemProcessOperationContext>();
        }

        [Test]
        public void Constructor_Considers_Null_Message_Sender_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseDataRequestsSubscriberFactory(null, _logger));
        }

        [Test]
        public void Constructor_Considers_Null_Logger_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseDataRequestsSubscriberFactory(_publisher, null));
        }

        [Test]
        public void Build_Returns_Non_Null()
        {
            var factory = new UniverseDataRequestsSubscriberFactory(_publisher, _logger);

            var result = factory.Build(_ctx);

            Assert.IsNotNull(result);
        }
    }
}
