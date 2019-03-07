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

namespace Surveillance.Engine.DataCoordinator.Tests.Queues
{
    [TestFixture]
    public class QueueSubscriberTests
    {
        private IDataVerifier _dataVerifier;
        private IAutoSchedule _autoSchedule;
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private IMessageBusSerialiser _serialiser;
        private ISystemProcessContext _systemProcessContext;
        private ISystemProcessOperationContext _systemProcessOperationContext;
        private ILogger<QueueAutoscheduleSubscriber> _logger;

        [SetUp]
        public void Setup()
        {
            _dataVerifier = A.Fake<IDataVerifier>();
            _autoSchedule = A.Fake<IAutoSchedule>();
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _serialiser = new MessageBusSerialiser();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _systemProcessOperationContext = A.Fake<ISystemProcessOperationContext>();
            _logger = new NullLogger<QueueAutoscheduleSubscriber>();

            A
                .CallTo(() => _systemProcessContext.CreateAndStartOperationContext())
                .Returns(_systemProcessOperationContext);
        }

        [Test]
        public void Constructor_AwsQueueClient_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueAutoscheduleSubscriber(_dataVerifier, _autoSchedule, null, _awsConfiguration, _serialiser, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_AwsConfiguration_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueAutoscheduleSubscriber(_dataVerifier, _autoSchedule, _awsQueueClient, null, _serialiser, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_Serialiser_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueAutoscheduleSubscriber(_dataVerifier, _autoSchedule, _awsQueueClient, _awsConfiguration, null, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_SystmeProcessContext_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueAutoscheduleSubscriber(_dataVerifier, _autoSchedule, _awsQueueClient, _awsConfiguration, _serialiser, null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueAutoscheduleSubscriber(_dataVerifier, _autoSchedule, _awsQueueClient, _awsConfiguration, _serialiser, _systemProcessContext, null));
        }

        [Test]
        public void Initiate_Subscribes_To_Queue()
        {
            A.CallTo(() => _awsConfiguration.UploadCoordinatorQueueName).Returns("a-queue-name");

            var subscriber = new QueueAutoscheduleSubscriber(_dataVerifier, _autoSchedule, _awsQueueClient, _awsConfiguration, _serialiser, _systemProcessContext, _logger);

            subscriber.Initiate();

            A
                .CallTo(() => 
                    _awsQueueClient.SubscribeToQueueAsync(
                        "a-queue-name",
                        A<Func<string, string, Task>>.Ignored,
                        A<CancellationToken>.Ignored,
                        A<AwsResusableCancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ExecuteCoordinationMessage_Ends_Event_With_Error_If_Not_Deserialisable()
        {
            var subscriber = new QueueAutoscheduleSubscriber(_dataVerifier, _autoSchedule, _awsQueueClient, _awsConfiguration, _serialiser, _systemProcessContext, _logger);

            await subscriber.ExecuteCoordinationMessage("message-id", "not-a-upload-message");

            A
                .CallTo(() => _systemProcessOperationContext.EndEventWithError(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ExecuteCoordinationMessage_Calls_AnalyseFileId_For_Valid_UploadMessage()
        {
            var subscriber = new QueueAutoscheduleSubscriber(_dataVerifier, _autoSchedule, _awsQueueClient, _awsConfiguration, _serialiser, _systemProcessContext, _logger);
            var uploadMessage = new AutoScheduleMessage {};
            var message = _serialiser.Serialise<AutoScheduleMessage>(uploadMessage);

            await subscriber.ExecuteCoordinationMessage("message-id", message);

            A
                .CallTo(() => _dataVerifier.Scan())
                .MustHaveHappenedOnceExactly();
        }
    }
}
