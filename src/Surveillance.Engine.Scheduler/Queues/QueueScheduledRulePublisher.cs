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
    /// The queue scheduled rule publisher.
    /// </summary>
    public class QueueScheduledRulePublisher : IQueueScheduledRulePublisher
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
        private readonly ILogger<IQueueScheduledRulePublisher> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueScheduledRulePublisher"/> class.
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
        public QueueScheduledRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<IQueueScheduledRulePublisher> logger)
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
                this.logger.LogInformation("asked to publish a rescheduled request that was null or empty");
                return;
            }

            var messageBusCts = new CancellationTokenSource();

            try
            {
                this.logger.LogInformation($"dispatching to {this.awsConfiguration.ScheduledRuleQueueName}");

                await this.awsQueueClient
                    .SendToQueue(
                        this.awsConfiguration.ScheduledRuleQueueName,
                        request.JsonSqsMessage,
                        messageBusCts.Token)
                    ;

                this.logger.LogInformation($"finished dispatching to {this.awsConfiguration.ScheduledRuleQueueName}");
            }
            catch (Exception e)
            {
                this.logger.LogError(
                    $"exception sending message '{request.JsonSqsMessage}' to queue '{this.awsConfiguration.ScheduledRuleQueueName}'. Error was {e.Message} {e.InnerException?.Message}",
                    e);
            }
        }
    }
}