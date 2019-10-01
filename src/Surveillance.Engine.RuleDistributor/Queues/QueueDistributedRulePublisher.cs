namespace Surveillance.Engine.RuleDistributor.Queues
{
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Scheduling.Interfaces;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

    /// <summary>
    /// The queue distributed rule publisher.
    /// </summary>
    public class QueueDistributedRulePublisher : IQueueDistributedRulePublisher
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
        /// The message bus serializer.
        /// </summary>
        private readonly IScheduledExecutionMessageBusSerialiser messageBusSerialiser;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<QueueDistributedRulePublisher> logger;

        /// <summary>
        /// The message bus cancellation token source.
        /// </summary>
        private CancellationTokenSource messageBusCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueDistributedRulePublisher"/> class.
        /// </summary>
        /// <param name="awsQueueClient">
        /// The aws queue client.
        /// </param>
        /// <param name="awsConfiguration">
        /// The aws configuration.
        /// </param>
        /// <param name="messageBusSerialiser">
        /// The message bus serializer.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public QueueDistributedRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ILogger<QueueDistributedRulePublisher> logger)
        {
            this.awsQueueClient = awsQueueClient;
            this.awsConfiguration = awsConfiguration;
            this.messageBusSerialiser = messageBusSerialiser;
            this.logger = logger;
        }

        /// <summary>
        /// The schedule execution.
        /// </summary>
        /// <param name="distributedExecution">
        /// The distributed execution.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ScheduleExecution(ScheduledExecution distributedExecution)
        {
            var serialisedDistributedExecution = this.messageBusSerialiser.SerialiseScheduledExecution(distributedExecution);

            this.logger.LogInformation($"dispatching distribute message to queue - {serialisedDistributedExecution}");

            this.messageBusCancellationTokenSource = this.messageBusCancellationTokenSource ?? new CancellationTokenSource();

            await 
                this
                    .awsQueueClient
                    .SendToQueue(
                        this.awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                        serialisedDistributedExecution,
                        this.messageBusCancellationTokenSource.Token)
                    ;
        }
    }
}