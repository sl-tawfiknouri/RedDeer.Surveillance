﻿namespace Surveillance.Engine.Rules.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Queues;
    using SharedKernel.Contracts.Queues.Interfaces;

    using Surveillance.Engine.Rules.Queues.Interfaces;

    public class QueueDataSynchroniserRequestPublisher : IQueueDataSynchroniserRequestPublisher
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueDataSynchroniserRequestPublisher> _logger;

        private readonly IThirdPartyDataRequestSerialiser _serialiser;

        public QueueDataSynchroniserRequestPublisher(
            IAwsConfiguration awsConfiguration,
            IAwsQueueClient awsQueueClient,
            IThirdPartyDataRequestSerialiser serialiser,
            ILogger<QueueDataSynchroniserRequestPublisher> logger)
        {
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(string ruleRunId)
        {
            if (string.IsNullOrWhiteSpace(ruleRunId))
            {
                this._logger.LogError("received a null or empty rule run id");
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var message = new ThirdPartyDataRequestMessage { SystemProcessOperationId = ruleRunId };
            var serialisedMessage = this._serialiser.Serialise(message);

            try
            {
                await this._awsQueueClient.SendToQueue(
                    this._awsConfiguration.DataSynchroniserRequestQueueName,
                    serialisedMessage,
                    messageBusCts.Token);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"encountered an error when sending rule run id {ruleRunId} to the data requests queue.");
            }
        }
    }
}