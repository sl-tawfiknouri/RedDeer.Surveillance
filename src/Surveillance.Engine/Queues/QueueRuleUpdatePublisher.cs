namespace Surveillance.Engine.Rules.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService;
    using RedDeer.Contracts.SurveillanceService.Interfaces;

    using Surveillance.Engine.Rules.Queues.Interfaces;

    public class QueueRuleUpdatePublisher : IQueueRuleUpdatePublisher
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueRuleUpdatePublisher> _logger;

        private readonly IMessageBusSerialiser _serialiser;

        public QueueRuleUpdatePublisher(
            IAwsConfiguration awsConfiguration,
            IAwsQueueClient awsQueueClient,
            IMessageBusSerialiser serialiser,
            ILogger<QueueRuleUpdatePublisher> logger)
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

            this._logger.LogInformation($"received request to send to rule update publisher for rule run {ruleRunId}");

            var messageBusCts = new CancellationTokenSource();

            var message = new TestRuleRunUpdateMessage { TestRuleRunId = ruleRunId };

            var serialisedMessage = this._serialiser.Serialise(message);

            try
            {
                this._logger.LogInformation(
                    $"sending {serialisedMessage} to {this._awsConfiguration.TestRuleRunUpdateQueueName} for {ruleRunId}");

                await this._awsQueueClient.SendToQueue(
                    this._awsConfiguration.TestRuleRunUpdateQueueName,
                    serialisedMessage,
                    messageBusCts.Token);

                this._logger.LogInformation(
                    $"sent {serialisedMessage} to {this._awsConfiguration.TestRuleRunUpdateQueueName} for {ruleRunId}");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"encountered an error {e.Message} {e.InnerException?.Message} when sending rule run id {ruleRunId} to rule run updates queue.",
                    e);
            }
        }
    }
}