namespace Surveillance.Engine.RuleDistributor.Queues
{
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Scheduling.Interfaces;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

    public class QueueDistributedRulePublisher : IQueueDistributedRulePublisher
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueDistributedRulePublisher> _logger;

        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private CancellationTokenSource _messageBusCts;

        public QueueDistributedRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ILogger<QueueDistributedRulePublisher> logger)
        {
            this._awsQueueClient = awsQueueClient;
            this._awsConfiguration = awsConfiguration;
            this._messageBusSerialiser = messageBusSerialiser;
            this._logger = logger;
        }

        public async Task ScheduleExecution(ScheduledExecution distributedExecution)
        {
            var serialisedDistributedExecution =
                this._messageBusSerialiser.SerialiseScheduledExecution(distributedExecution);

            this._logger.LogInformation($"dispatching distribute message to queue - {serialisedDistributedExecution}");

            this._messageBusCts = this._messageBusCts ?? new CancellationTokenSource();

            await this._awsQueueClient.SendToQueue(
                this._awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                serialisedDistributedExecution,
                this._messageBusCts.Token);
        }
    }
}