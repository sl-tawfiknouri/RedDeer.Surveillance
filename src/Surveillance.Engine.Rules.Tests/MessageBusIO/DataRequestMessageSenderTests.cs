namespace Surveillance.Engine.Rules.Tests.MessageBusIO
{
    using System.Threading;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using SharedKernel.Contracts.Queues;
    using SharedKernel.Contracts.Queues.Interfaces;

    using Surveillance.DataLayer.Configuration;
    using Surveillance.Engine.Rules.Queues;

    [TestFixture]
    public class DataRequestMessageSenderTests
    {
        private IAwsConfiguration _awsConfiguration;

        private IAwsQueueClient _awsQueueClient;

        private ILogger<QueueDataSynchroniserRequestPublisher> _logger;

        private IThirdPartyDataRequestSerialiser _serialiser;

        [Test]
        public async Task Send_NullMessage_DoesNotSendToQueue()
        {
            var messageSender = this.BuildSender();

            await messageSender.Send(null);

            A.CallTo(() => this._serialiser.Serialise(A<ThirdPartyDataRequestMessage>.Ignored)).MustNotHaveHappened();
            A.CallTo(
                () => this._awsQueueClient.SendToQueue(
                    A<string>.Ignored,
                    A<string>.Ignored,
                    A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        [Explicit]
        public async Task Send_SubmitsMessageToQueue()
        {
            var configuration = new DataLayerConfiguration
                                    {
                                        DataSynchroniserRequestQueueName =
                                            "dev-surveillance-reddeer-data-synchronizer-request"
                                    };
            var queueClient = new AwsQueueClient(null);
            var serialiser = new ThirdPartyDataRequestSerialiser();
            var messageSender = new QueueDataSynchroniserRequestPublisher(
                configuration,
                queueClient,
                serialiser,
                this._logger);

            await messageSender.Send("1");
        }

        [SetUp]
        public void Setup()
        {
            this._awsQueueClient = A.Fake<IAwsQueueClient>();
            this._awsConfiguration = A.Fake<IAwsConfiguration>();
            this._logger = A.Fake<ILogger<QueueDataSynchroniserRequestPublisher>>();
            this._serialiser = A.Fake<IThirdPartyDataRequestSerialiser>();
        }

        private QueueDataSynchroniserRequestPublisher BuildSender()
        {
            return new QueueDataSynchroniserRequestPublisher(
                this._awsConfiguration,
                this._awsQueueClient,
                this._serialiser,
                this._logger);
        }
    }
}