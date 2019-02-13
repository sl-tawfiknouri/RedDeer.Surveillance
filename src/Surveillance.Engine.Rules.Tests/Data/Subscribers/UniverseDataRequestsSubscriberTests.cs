using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Tests.Data.Subscribers
{
    [TestFixture]
    public class UniverseDataRequestsSubscriberTests
    {
        private ISystemProcessOperationContext _operationContext;
        private IDataRequestMessageSender _dataRequestMessageSender;
        private ILogger<UniverseDataRequestsSubscriber> _logger;

        [SetUp]
        public void Setup()
        {
            _operationContext = A.Fake<ISystemProcessOperationContext>();
            _dataRequestMessageSender = A.Fake<IDataRequestMessageSender>();
            _logger = A.Fake<ILogger<UniverseDataRequestsSubscriber>>();
        }

        [Test]
        public void Constructor_Considers_Null_Operation_Context_To_Be_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseDataRequestsSubscriber(null, _dataRequestMessageSender, _logger));
        }

        [Test]
        public void Constructor_Considers_Null_Message_Sender_To_Be_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseDataRequestsSubscriber(_operationContext, null, _logger));
        }

        [Test]
        public void Constructor_Considers_Null_Logger_To_Be_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseDataRequestsSubscriber(_operationContext, _dataRequestMessageSender, null));
        }

        [Test]
        public void OnNext_Eschaton_And_Submit_Requests_Calls_DataRequestMessageSender()
        {
            var subscriber = new UniverseDataRequestsSubscriber(_operationContext, _dataRequestMessageSender, _logger);
            var events = new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object());

            subscriber.SubmitRequest();
            subscriber.OnNext(events);

            A.CallTo(() => _dataRequestMessageSender.Send(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_Eschaton_And_No_Submit_Does_Not_Requests_Calls_DataRequestMessageSender()
        {
            var subscriber = new UniverseDataRequestsSubscriber(_operationContext, _dataRequestMessageSender, _logger);
            var events = new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object());

            subscriber.OnNext(events);

            A.CallTo(() => _dataRequestMessageSender.Send(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_Genesis_And_Submit_Does_Requests_Calls_DataRequestMessageSender()
        {
            var subscriber = new UniverseDataRequestsSubscriber(_operationContext, _dataRequestMessageSender, _logger);
            var events = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object());

            subscriber.SubmitRequest();
            subscriber.OnNext(events);

            A.CallTo(() => _dataRequestMessageSender.Send(A<string>.Ignored)).MustNotHaveHappened();
        }
    }
}
