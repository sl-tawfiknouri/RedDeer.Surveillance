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

    /// <summary>
    /// The queue schedule rule publisher.
    /// </summary>
    public class QueueScheduleRulePublisher : IQueueScheduleRulePublisher
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
        /// The serialiser.
        /// </summary>
        private readonly IScheduledExecutionMessageBusSerialiser serialiser;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<QueueScheduleRulePublisher> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueScheduleRulePublisher"/> class.
        /// </summary>
        /// <param name="awsQueueClient">
        /// The aws queue client.
        /// </param>
        /// <param name="awsConfiguration">
        /// The aws configuration.
        /// </param>
        /// <param name="serialiser">
        /// The serializer.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public QueueScheduleRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser serialiser,
            ILogger<QueueScheduleRulePublisher> logger)
        {
            this.awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this.awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this.serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Send(ScheduledExecution message)
        {
            if (message == null)
            {
                this.logger.LogWarning("was asked to send a null message. Will not be sending anything.");
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var serialisedMessage = this.serialiser.SerialiseScheduledExecution(message);

            try
            {
                this.logger.LogInformation($"dispatching to {this.awsConfiguration.ScheduledRuleQueueName}");

                await 
                    this
                        .awsQueueClient
                        .SendToQueue(this.awsConfiguration.ScheduledRuleQueueName, serialisedMessage, messageBusCts.Token)
                        ;

                this.logger.LogInformation($"finished dispatching to {this.awsConfiguration.ScheduledRuleQueueName}");
            }
            catch (Exception e)
            {
                this.logger.LogError(e, $"Exception sending message '{message}' to queue {this.awsConfiguration.ScheduledRuleQueueName}.");
            }
        }
    }
}