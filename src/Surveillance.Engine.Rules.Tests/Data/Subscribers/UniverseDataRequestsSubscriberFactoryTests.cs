using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Tests.Data.Subscribers
{
    [TestFixture]
    public class UniverseDataRequestsSubscriberFactoryTests
    {
        private IDataRequestMessageSender _messageSender;
        private ILogger<UniverseDataRequestsSubscriber> _logger;
        private ISystemProcessOperationContext _ctx;

        [SetUp]
        public void Setup()
        {
            _messageSender = A.Fake<IDataRequestMessageSender>();
            _logger = A.Fake<ILogger<UniverseDataRequestsSubscriber>>();
            _ctx = A.Fake<ISystemProcessOperationContext>();
        }

        [Test]
        public void Constructor_Considers_Null_Message_Sender_To_Be_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseDataRequestsSubscriberFactory(null, _logger));
        }

        [Test]
        public void Constructor_Considers_Null_Logger_To_Be_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseDataRequestsSubscriberFactory(_messageSender, null));
        }

        [Test]
        public void Build_Returns_Non_Null()
        {
            var factory = new UniverseDataRequestsSubscriberFactory(_messageSender, _logger);

            var result = factory.Build(_ctx);

            Assert.IsNotNull(result);
        }
    }
}
