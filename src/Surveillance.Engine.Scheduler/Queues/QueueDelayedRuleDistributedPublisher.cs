namespace Surveillance.Engine.Scheduler.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Scheduler.Queues.Interfaces;

    public class QueueDelayedRuleDistributedPublisher : IQueueDelayedRuleDistributedPublisher
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueDelayedRuleDistributedPublisher> _logger;

        public QueueDelayedRuleDistributedPublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<QueueDelayedRuleDistributedPublisher> logger)
        {
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Publish(AdHocScheduleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.JsonSqsMessage))
            {
                this._logger?.LogInformation("asked to publish a null or empty request message");
                return;
            }

            var cancellationToken = new CancellationTokenSource();

            try
            {
                this._logger.LogInformation(
                    $"publishing message to {this._awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
                await this._awsQueueClient.SendToQueue(
                    this._awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                    request.JsonSqsMessage,
                    cancellationToken.Token);
                this._logger.LogInformation(
                    $"published message to {this._awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"exception when publishing '{request.JsonSqsMessage}' to queue {this._awsConfiguration.ScheduleRuleDistributedWorkQueueName} {e.Message} {e.InnerException?.Message}",
                    e);
            }
        }
    }
}