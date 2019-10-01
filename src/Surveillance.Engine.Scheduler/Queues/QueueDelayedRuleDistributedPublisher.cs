namespace Surveillance.Engine.Scheduler.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Scheduler.Queues.Interfaces;

    /// <summary>
    /// The queue delayed rule distributed publisher.
    /// </summary>
    public class QueueDelayedRuleDistributedPublisher : IQueueDelayedRuleDistributedPublisher
    {
        /// <summary>
        /// The aws configuration.
        /// </summary>
        private readonly IAwsConfiguration awsConfiguration;

        /// <summary>
        /// The aws queue client.
        /// </summary>
        private readonly IAwsQueueClient awsQueueClient;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<QueueDelayedRuleDistributedPublisher> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueDelayedRuleDistributedPublisher"/> class.
        /// </summary>
        /// <param name="awsQueueClient">
        /// The aws queue client.
        /// </param>
        /// <param name="awsConfiguration">
        /// The aws configuration.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public QueueDelayedRuleDistributedPublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<QueueDelayedRuleDistributedPublisher> logger)
        {
            this.awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this.awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Publish(AdHocScheduleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.JsonSqsMessage))
            {
                this.logger?.LogInformation("asked to publish a null or empty request message");
                return;
            }

            var cancellationToken = new CancellationTokenSource();

            try
            {
                this.logger.LogInformation(
                    $"publishing message to {this.awsConfiguration.ScheduleRuleDistributedWorkQueueName}");

                await this.awsQueueClient
                    .SendToQueue(
                        this.awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                        request.JsonSqsMessage,
                        cancellationToken.Token)
                    ;

                this.logger.LogInformation(
                    $"published message to {this.awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
            }
            catch (Exception e)
            {
                this.logger.LogError(
                    $"exception when publishing '{request.JsonSqsMessage}' to queue {this.awsConfiguration.ScheduleRuleDistributedWorkQueueName} {e.Message} {e.InnerException?.Message}",
                    e);
            }
        }
    }
}