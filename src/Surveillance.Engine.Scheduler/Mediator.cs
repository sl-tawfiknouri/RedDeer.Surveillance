using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Scheduler.Interfaces;
using Surveillance.Engine.Scheduler.Queues.Interfaces;

namespace Surveillance.Engine.Scheduler
{
    public class Mediator : IRuleSchedulerMediator
    {
        private readonly IQueueRuleSchedulerSubscriber _ruleSchedulerSubscriber;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IQueueRuleSchedulerSubscriber ruleSchedulerSubscriber,
            ILogger<Mediator> logger)
        {
            _ruleSchedulerSubscriber = ruleSchedulerSubscriber ?? throw new ArgumentNullException(nameof(ruleSchedulerSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"initiating");
            _ruleSchedulerSubscriber.Initiate();
            _logger.LogInformation($"completed initiation");
        }

        public void Terminate()
        {
            _logger.LogInformation($"terminating");
            _ruleSchedulerSubscriber.Terminate();
            _logger.LogInformation($"completed termination");
        }
    }
}
