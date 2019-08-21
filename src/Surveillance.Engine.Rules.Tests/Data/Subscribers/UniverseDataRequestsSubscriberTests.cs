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
    public class UniverseDataRequestsSubscriberTests
    {
        private ILogger<UniverseDataRequestsSubscriber> _logger;

        private ISystemProcessOperationContext _operationContext;

        private IQueueDataSynchroniserRequestPublisher _queueDataSynchroniserRequestPublisher;

        [Test]
        public void Constructor_Considers_Null_Logger_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniverseDataRequestsSubscriber(
                    this._operationContext,
                    this._queueDataSynchroniserRequestPublisher,
                    null));
        }

        [Test]
        public void Constructor_Considers_Null_Message_Sender_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniverseDataRequestsSubscriber(this._operationContext, null, this._logger));
        }

        [Test]
        public void Constructor_Considers_Null_Operation_Context_To_Be_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniverseDataRequestsSubscriber(
                    null,
                    this._queueDataSynchroniserRequestPublisher,
                    this._logger));
        }

        [Test]
        public void OnNext_Eschaton_And_No_Submit_Does_Not_Requests_Calls_DataRequestMessageSender()
        {
            var subscriber = new UniverseDataRequestsSubscriber(
                this._operationContext,
                this._queueDataSynchroniserRequestPublisher,
                this._logger);

            subscriber.DispatchIfSubmitRequest();

            A.CallTo(() => this._queueDataSynchroniserRequestPublisher.Send(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_Eschaton_And_Submit_Requests_Calls_DataRequestMessageSender()
        {
            var subscriber = new UniverseDataRequestsSubscriber(
                this._operationContext,
                this._queueDataSynchroniserRequestPublisher,
                this._logger);

            subscriber.SubmitRequest();
            subscriber.DispatchIfSubmitRequest();

            A.CallTo(() => this._queueDataSynchroniserRequestPublisher.Send(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._operationContext = A.Fake<ISystemProcessOperationContext>();
            this._queueDataSynchroniserRequestPublisher = A.Fake<IQueueDataSynchroniserRequestPublisher>();
            this._logger = A.Fake<ILogger<UniverseDataRequestsSubscriber>>();
        }
    }
}