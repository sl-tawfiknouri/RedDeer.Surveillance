using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Scheduler.Queues.Interfaces;

namespace Surveillance.Engine.Scheduler.Queues
{
    public class QueueRuleSchedulerSubscriber : IQueueRuleSchedulerSubscriber
    {
        private readonly ILogger<QueueRuleSchedulerSubscriber> _logger;

        public QueueRuleSchedulerSubscriber(
            ILogger<QueueRuleSchedulerSubscriber> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"initiating");



            _logger.LogInformation($"completed initiation");
        }

        public void Terminate()
        {
            _logger.LogInformation($"terminating");



            _logger.LogInformation($"completed termination");
        }
    }
}
