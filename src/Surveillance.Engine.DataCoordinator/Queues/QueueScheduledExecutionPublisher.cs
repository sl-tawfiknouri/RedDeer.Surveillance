namespace Surveillance.Engine.DataCoordinator.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Scheduling.Interfaces;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

    public class QueueScheduleRulePublisher : IQueueScheduleRulePublisher
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueScheduleRulePublisher> _logger;

        private readonly IScheduledExecutionMessageBusSerialiser _serialiser;

        public QueueScheduleRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser serialiser,
            ILogger<QueueScheduleRulePublisher> logger)
        {
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(ScheduledExecution message)
        {
            if (message == null)
            {
                this._logger.LogWarning("was asked to send a null message. Will not be sending anything.");
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var serialisedMessage = this._serialiser.SerialiseScheduledExecution(message);

            try
            {
                this._logger.LogInformation($"dispatching to {this._awsConfiguration.ScheduledRuleQueueName}");
                await this._awsQueueClient.SendToQueue(
                    this._awsConfiguration.ScheduledRuleQueueName,
                    serialisedMessage,
                    messageBusCts.Token);
                this._logger.LogInformation($"finished dispatching to {this._awsConfiguration.ScheduledRuleQueueName}");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"Exception sending message '{message}' to queue {this._awsConfiguration.ScheduledRuleQueueName}. Error was {e.Message}");
            }
        }
    }
}