﻿using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Configuration;
using Surveillance.Engine.Rules.Queues;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Engine.Rules.Tests.MessageBusIO
{
    [TestFixture]
    public class DataRequestMessageSenderTests
    {
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private ILogger<QueueDataSynchroniserRequestPublisher> _logger;
        private IThirdPartyDataRequestSerialiser _serialiser;

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _logger = A.Fake<ILogger<QueueDataSynchroniserRequestPublisher>>();
            _serialiser = A.Fake<IThirdPartyDataRequestSerialiser>();
        }

        [Test]
        public async Task Send_NullMessage_DoesNotSendToQueue()
        {
            var messageSender = BuildSender();

            await messageSender.Send(null);

            A.CallTo(() => _serialiser.Serialise(A<ThirdPartyDataRequestMessage>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _awsQueueClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
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
            var messageSender = new QueueDataSynchroniserRequestPublisher(configuration, queueClient, serialiser, _logger);

            await messageSender.Send("1");
        }

        private QueueDataSynchroniserRequestPublisher BuildSender()
        {
            return new QueueDataSynchroniserRequestPublisher(_awsConfiguration, _awsQueueClient, _serialiser, _logger);
        }
    }
}