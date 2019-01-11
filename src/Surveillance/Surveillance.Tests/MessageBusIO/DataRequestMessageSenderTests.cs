using System.Threading;
using System.Threading.Tasks;
using DomainV2.DTO;
using DomainV2.DTO.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Configuration;
using Surveillance.MessageBusIO;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Tests.MessageBusIO
{
    [TestFixture]
    public class DataRequestMessageSenderTests
    {
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private ILogger<DataRequestMessageSender> _logger;
        private IThirdPartyDataRequestSerialiser _serialiser;

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _logger = A.Fake<ILogger<DataRequestMessageSender>>();
            _serialiser = A.Fake<IThirdPartyDataRequestSerialiser>();
        }

        [Test]
        public async Task Send_NullMessage_DoesNotSendToQueue()
        {
            var messageSender = BuildSender();

            await messageSender.Send(null);

            A
                .CallTo(() => _serialiser.Serialise(A<ThirdPartyDataRequestMessage>.Ignored))
                .MustNotHaveHappened();
            A
                .CallTo(() => _awsQueueClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        [Explicit]
        public async Task Send_SubmitsMessageToQueue()
        {
            var configuration = new DataLayerConfiguration
            {
                DataSynchroniserRequestQueueName = "dev-surveillance-reddeer-data-synchronizer-request"
            };
            var queueClient = new AwsQueueClient(null);
            var serialiser = new ThirdPartyDataRequestSerialiser();
            var messageSender = new DataRequestMessageSender(configuration, queueClient, serialiser, _logger);

            await messageSender.Send("1");
        }

        private DataRequestMessageSender BuildSender()
        {
            return new DataRequestMessageSender(_awsConfiguration, _awsQueueClient, _serialiser, _logger);
        }
    }
}
