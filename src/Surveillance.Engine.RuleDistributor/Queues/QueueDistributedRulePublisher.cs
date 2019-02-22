﻿using System.Threading;
using System.Threading.Tasks;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.RuleDistributor.Queues.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Engine.RuleDistributor.Queues
{
    public class QueueDistributedRulePublisher : IQueueDistributedRulePublisher
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly ILogger<QueueDistributedRulePublisher> _logger;

        private CancellationTokenSource _messageBusCts;

        public QueueDistributedRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ILogger<QueueDistributedRulePublisher> logger)
        {
            _awsQueueClient = awsQueueClient;
            _awsConfiguration = awsConfiguration;
            _messageBusSerialiser = messageBusSerialiser;
            _logger = logger;
        }

        public async Task ScheduleExecution(ScheduledExecution distributedExecution)
        {
            var serialisedDistributedExecution =
                _messageBusSerialiser.SerialiseScheduledExecution(distributedExecution);

            _logger.LogInformation($"QueueDistributedRulePublisher - dispatching distribute message to queue - {serialisedDistributedExecution}");

            _messageBusCts = _messageBusCts ?? new CancellationTokenSource();

            await _awsQueueClient.SendToQueue(
                _awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                serialisedDistributedExecution,
                _messageBusCts.Token);
        }
    }
}