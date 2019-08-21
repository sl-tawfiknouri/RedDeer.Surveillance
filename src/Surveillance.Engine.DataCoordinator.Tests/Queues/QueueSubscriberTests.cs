namespace Surveillance.Engine.DataCoordinator.Tests.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService;
    using RedDeer.Contracts.SurveillanceService.Interfaces;

    using SharedKernel.Contracts.Queues;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Queues;

    [TestFixture]
    public class QueueSubscriberTests
    {
        private IAutoSchedule _autoSchedule;

        private IAwsConfiguration _awsConfiguration;

        private IAwsQueueClient _awsQueueClient;

        private IDataVerifier _dataVerifier;

        private ILogger<QueueAutoscheduleSubscriber> _logger;

        private IMessageBusSerialiser _serialiser;

        private ISystemProcessContext _systemProcessContext;

        private ISystemProcessOperationContext _systemProcessOperationContext;

        [Test]
        public void Constructor_AwsConfiguration_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoscheduleSubscriber(
                    this._dataVerifier,
                    this._autoSchedule,
                    this._awsQueueClient,
                    null,
                    this._serialiser,
                    this._systemProcessContext,
                    this._logger));
        }

        [Test]
        public void Constructor_AwsQueueClient_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoscheduleSubscriber(
                    this._dataVerifier,
                    this._autoSchedule,
                    null,
                    this._awsConfiguration,
                    this._serialiser,
                    this._systemProcessContext,
                    this._logger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoscheduleSubscriber(
                    this._dataVerifier,
                    this._autoSchedule,
                    this._awsQueueClient,
                    this._awsConfiguration,
                    this._serialiser,
                    this._systemProcessContext,
                    null));
        }

        [Test]
        public void Constructor_Serialiser_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoscheduleSubscriber(
                    this._dataVerifier,
                    this._autoSchedule,
                    this._awsQueueClient,
                    this._awsConfiguration,
                    null,
                    this._systemProcessContext,
                    this._logger));
        }

        [Test]
        public void Constructor_SystmeProcessContext_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoscheduleSubscriber(
                    this._dataVerifier,
                    this._autoSchedule,
                    this._awsQueueClient,
                    this._awsConfiguration,
                    this._serialiser,
                    null,
                    this._logger));
        }

        [Test]
        public async Task ExecuteCoordinationMessage_Calls_AnalyseFileId_For_Valid_UploadMessage()
        {
            var subscriber = new QueueAutoscheduleSubscriber(
                this._dataVerifier,
                this._autoSchedule,
                this._awsQueueClient,
                this._awsConfiguration,
                this._serialiser,
                this._systemProcessContext,
                this._logger);
            var uploadMessage = new AutoScheduleMessage();
            var message = this._serialiser.Serialise(uploadMessage);

            await subscriber.ExecuteCoordinationMessage("message-id", message);

            A.CallTo(() => this._dataVerifier.Scan()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ExecuteCoordinationMessage_Ends_Event_With_Error_If_Not_Deserialisable()
        {
            var subscriber = new QueueAutoscheduleSubscriber(
                this._dataVerifier,
                this._autoSchedule,
                this._awsQueueClient,
                this._awsConfiguration,
                this._serialiser,
                this._systemProcessContext,
                this._logger);

            await subscriber.ExecuteCoordinationMessage("message-id", "not-a-upload-message");

            A.CallTo(() => this._systemProcessOperationContext.EndEventWithError(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Initiate_Subscribes_To_Queue()
        {
            A.CallTo(() => this._awsConfiguration.UploadCoordinatorQueueName).Returns("a-queue-name");

            var subscriber = new QueueAutoscheduleSubscriber(
                this._dataVerifier,
                this._autoSchedule,
                this._awsQueueClient,
                this._awsConfiguration,
                this._serialiser,
                this._systemProcessContext,
                this._logger);

            subscriber.Initiate();

            A.CallTo(
                () => this._awsQueueClient.SubscribeToQueueAsync(
                    "a-queue-name",
                    A<Func<string, string, Task>>.Ignored,
                    A<CancellationToken>.Ignored,
                    A<AwsResusableCancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._dataVerifier = A.Fake<IDataVerifier>();
            this._autoSchedule = A.Fake<IAutoSchedule>();
            this._awsQueueClient = A.Fake<IAwsQueueClient>();
            this._awsConfiguration = A.Fake<IAwsConfiguration>();
            this._serialiser = new MessageBusSerialiser();
            this._systemProcessContext = A.Fake<ISystemProcessContext>();
            this._systemProcessOperationContext = A.Fake<ISystemProcessOperationContext>();
            this._logger = new NullLogger<QueueAutoscheduleSubscriber>();

            A.CallTo(() => this._systemProcessContext.CreateAndStartOperationContext())
                .Returns(this._systemProcessOperationContext);
        }
    }
}