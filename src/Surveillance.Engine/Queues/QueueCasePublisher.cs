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

    public class QueueCasePublisher : IQueueCasePublisher
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueCasePublisher> _logger;

        private readonly IMessageBusSerialiser _serialiser;

        public QueueCasePublisher(
            IMessageBusSerialiser serialiser,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<QueueCasePublisher> logger)
        {
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(CaseMessage message)
        {
            if (message == null)
            {
                this._logger.LogWarning("tried to send a null case message. Did not send to AWS.");
                return;
            }

            var caseMessage = this._serialiser.Serialise(message);
            var messageBusCts = new CancellationTokenSource();

            try
            {
                this._logger.LogInformation($"Send | about to dispatch case {message.RuleBreachId} (id) to AWS queue");
                await this._awsQueueClient.SendToQueue(
                    this._awsConfiguration.CaseMessageQueueName,
                    caseMessage,
                    messageBusCts.Token);
                this._logger.LogInformation($"Send | now dispatched case with id {message.RuleBreachId}");
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Exception sending message '{caseMessage}' to bus on queue {this._awsConfiguration.CaseMessageQueueName}.");
            }
        }
    }
}