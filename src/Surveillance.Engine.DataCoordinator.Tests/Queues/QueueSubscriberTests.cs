using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.SurveillanceService.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Tests.Queues
{
    [TestFixture]
    public class QueueSubscriberTests
    {
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private IMessageBusSerialiser _serialiser;
        private ISystemProcessContext _systemProcessContext;
        private ILogger<QueueSubscriber> _logger;

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _serialiser = A.Fake<IMessageBusSerialiser>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _logger = new NullLogger<QueueSubscriber>();
        }

        [Test]
        public void Constructor_AwsQueueClient_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueSubscriber(null, _awsConfiguration, _serialiser, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_AwsConfiguration_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueSubscriber(_awsQueueClient, null, _serialiser, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_Serialiser_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueSubscriber(_awsQueueClient, _awsConfiguration, null, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_SystmeProcessContext_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueSubscriber(_awsQueueClient, _awsConfiguration, _serialiser, null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QueueSubscriber(_awsQueueClient, _awsConfiguration, _serialiser, _systemProcessContext, null));
        }

        [Test]
        public void Initiate_Subscribes_To_Queue()
        {
            A.CallTo(() => _awsConfiguration.UploadCoordinatorQueueName).Returns("a-queue-name");

            var subscriber = new QueueSubscriber(_awsQueueClient, _awsConfiguration, _serialiser, _systemProcessContext, _logger);

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


    }
}
