using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Scheduler.Interfaces;
using Surveillance.Engine.Scheduler.Queues.Interfaces;

namespace Surveillance.Engine.Scheduler
{
    public class Mediator : IRuleSchedulerMediator
    {
        private readonly IQueueDelayedRuleSchedulerSubscriber _delayedRuleSchedulerSubscriber;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IQueueDelayedRuleSchedulerSubscriber delayedRuleSchedulerSubscriber,
            ILogger<Mediator> logger)
        {
            _delayedRuleSchedulerSubscriber = delayedRuleSchedulerSubscriber ?? throw new ArgumentNullException(nameof(delayedRuleSchedulerSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"initiating");
            _delayedRuleSchedulerSubscriber.Initiate();
            _logger.LogInformation($"completed initiation");
        }

        public void Terminate()
        {
            _logger.LogInformation($"terminating");
            _delayedRuleSchedulerSubscriber.Terminate();
            _logger.LogInformation($"completed termination");
        }
    }
}
