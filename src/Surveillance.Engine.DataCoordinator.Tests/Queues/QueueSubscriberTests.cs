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

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The queue subscriber tests.
    /// </summary>
    [TestFixture]
    public class QueueSubscriberTests
    {
        /// <summary>
        /// The auto schedule.
        /// </summary>
        private IAutoSchedule autoSchedule;

        /// <summary>
        /// The aws configuration.
        /// </summary>
        private IAwsConfiguration awsConfiguration;

        /// <summary>
        /// The aws queue client.
        /// </summary>
        private IAwsQueueClient awsQueueClient;

        /// <summary>
        /// The data verifier.
        /// </summary>
        private IDataVerifier dataVerifier;

        /// <summary>
        /// The serializer.
        /// </summary>
        private IMessageBusSerialiser serialiser;

        /// <summary>
        /// The system process context.
        /// </summary>
        private ISystemProcessContext systemProcessContext;

        /// <summary>
        /// The system process operation context.
        /// </summary>
        private ISystemProcessOperationContext systemProcessOperationContext;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<QueueAutoScheduleSubscriber> logger;

        /// <summary>
        /// The constructor aws configuration null throws exception.
        /// </summary>
        [Test]
        public void ConstructorAwsConfigurationNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoScheduleSubscriber(
                    this.dataVerifier,
                    this.autoSchedule,
                    this.awsQueueClient,
                    null,
                    this.serialiser,
                    this.systemProcessContext,
                    this.logger));
        }

        /// <summary>
        /// The constructor aws queue client null throws exception.
        /// </summary>
        [Test]
        public void ConstructorAwsQueueClientNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoScheduleSubscriber(
                    this.dataVerifier,
                    this.autoSchedule,
                    null,
                    this.awsConfiguration,
                    this.serialiser,
                    this.systemProcessContext,
                    this.logger));
        }

        /// <summary>
        /// The constructor logger null throws exception.
        /// </summary>
        [Test]
        public void ConstructorLoggerNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoScheduleSubscriber(
                    this.dataVerifier,
                    this.autoSchedule,
                    this.awsQueueClient,
                    this.awsConfiguration,
                    this.serialiser,
                    this.systemProcessContext,
                    null));
        }

        /// <summary>
        /// The constructor serializer null throws exception.
        /// </summary>
        [Test]
        public void ConstructorSerialiserNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoScheduleSubscriber(
                    this.dataVerifier,
                    this.autoSchedule,
                    this.awsQueueClient,
                    this.awsConfiguration,
                    null,
                    this.systemProcessContext,
                    this.logger));
        }

        /// <summary>
        /// The constructor system process context null throws exception.
        /// </summary>
        [Test]
        public void ConstructorSystemProcessContextNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new QueueAutoScheduleSubscriber(
                    this.dataVerifier,
                    this.autoSchedule,
                    this.awsQueueClient,
                    this.awsConfiguration,
                    this.serialiser,
                    null,
                    this.logger));
        }

        /// <summary>
        /// The execute coordination message calls analyze file id for valid upload message.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task ExecuteCoordinationMessageCallsAnalyseFileIdForValidUploadMessage()
        {
            var subscriber = new QueueAutoScheduleSubscriber(
                this.dataVerifier,
                this.autoSchedule,
                this.awsQueueClient,
                this.awsConfiguration,
                this.serialiser,
                this.systemProcessContext,
                this.logger);
            var uploadMessage = new AutoScheduleMessage();
            var message = this.serialiser.Serialise(uploadMessage);

            await subscriber.ExecuteCoordinationMessageAsync("message-id", message);

            A.CallTo(() => this.dataVerifier.Scan()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The execute coordination message ends event with error if not able to deserialize.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task ExecuteCoordinationMessageEndsEventWithErrorIfNotDeserialisable()
        {
            var subscriber = new QueueAutoScheduleSubscriber(
                this.dataVerifier,
                this.autoSchedule,
                this.awsQueueClient,
                this.awsConfiguration,
                this.serialiser,
                this.systemProcessContext,
                this.logger);

            await subscriber.ExecuteCoordinationMessageAsync("message-id", "not-a-upload-message");

            A.CallTo(() => this.systemProcessOperationContext.EndEventWithError(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The initiate subscribes to queue.
        /// </summary>
        [Test]
        public void InitiateSubscribesToQueue()
        {
            A.CallTo(() => this.awsConfiguration.UploadCoordinatorQueueName).Returns("a-queue-name");

            var subscriber = new QueueAutoScheduleSubscriber(
                this.dataVerifier,
                this.autoSchedule,
                this.awsQueueClient,
                this.awsConfiguration,
                this.serialiser,
                this.systemProcessContext,
                this.logger);

            subscriber.Initiate();

            A.CallTo(
                () => this.awsQueueClient.SubscribeToQueueAsync(
                    "a-queue-name",
                    A<Func<string, string, Task>>.Ignored,
                    A<CancellationToken>.Ignored,
                    A<AwsResusableCancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.dataVerifier = A.Fake<IDataVerifier>();
            this.autoSchedule = A.Fake<IAutoSchedule>();
            this.awsQueueClient = A.Fake<IAwsQueueClient>();
            this.awsConfiguration = A.Fake<IAwsConfiguration>();
            this.serialiser = new MessageBusSerialiser();
            this.systemProcessContext = A.Fake<ISystemProcessContext>();
            this.systemProcessOperationContext = A.Fake<ISystemProcessOperationContext>();
            this.logger = new NullLogger<QueueAutoScheduleSubscriber>();

            A.CallTo(() => this.systemProcessContext.CreateAndStartOperationContext())
                .Returns(this.systemProcessOperationContext);
        }
    }
}