﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Engine.Rules.Queues
{
    public class QueueDataSynchroniserRequestPublisher : IQueueDataSynchroniserRequestPublisher
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<QueueDataSynchroniserRequestPublisher> _logger;
        private readonly IThirdPartyDataRequestSerialiser _serialiser;

        public QueueDataSynchroniserRequestPublisher(
            IAwsConfiguration awsConfiguration,
            IAwsQueueClient awsQueueClient,
            IThirdPartyDataRequestSerialiser serialiser,
            ILogger<QueueDataSynchroniserRequestPublisher> logger)
        {
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(string ruleRunId)
        {
            if (string.IsNullOrWhiteSpace(ruleRunId))
            {
                _logger.LogError($"QueueDataSynchroniserRequestPublisher received a null or empty rule run id");
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var message = new ThirdPartyDataRequestMessage {SystemProcessOperationId = ruleRunId};
            var serialisedMessage = _serialiser.Serialise(message);

            try
            {
                await _awsQueueClient.SendToQueue(
                    _awsConfiguration.DataSynchroniserRequestQueueName,
                    serialisedMessage,
                    messageBusCts.Token);
            }
            catch (Exception e)
            {
                _logger.LogError($"QueueDataSynchroniserRequestPublisher encountered an error {e.Message} {e.InnerException?.Message} when sending rule run id {ruleRunId} to the data requests queue.");
            }
        }
    }
}