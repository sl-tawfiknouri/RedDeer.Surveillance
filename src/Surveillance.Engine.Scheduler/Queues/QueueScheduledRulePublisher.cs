namespace Surveillance.Engine.Scheduler.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Scheduler.Queues.Interfaces;

    public class QueueScheduledRulePublisher : IQueueScheduledRulePublisher
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<IQueueScheduledRulePublisher> _logger;

        public QueueScheduledRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<IQueueScheduledRulePublisher> logger)
        {
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Publish(AdHocScheduleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.JsonSqsMessage))
            {
                this._logger.LogInformation("asked to publish a rescheduled request that was null or empty");
                return;
            }

            var messageBusCts = new CancellationTokenSource();

            try
            {
                this._logger.LogInformation($"dispatching to {this._awsConfiguration.ScheduledRuleQueueName}");
                await this._awsQueueClient.SendToQueue(
                    this._awsConfiguration.ScheduledRuleQueueName,
                    request.JsonSqsMessage,
                    messageBusCts.Token);
                this._logger.LogInformation($"finished dispatching to {this._awsConfiguration.ScheduledRuleQueueName}");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"exception sending message '{request.JsonSqsMessage}' to queue '{this._awsConfiguration.ScheduledRuleQueueName}'. Error was {e.Message} {e.InnerException?.Message}",
                    e);
            }
        }
    }
}