using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.RuleDistributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

namespace Surveillance.Engine.RuleDistributor
{
    public class Mediator : IRuleDistributorMediator
    {
        private readonly IQueueReddeerDistributedRuleSubscriber _distributedRuleSubscriber;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IQueueReddeerDistributedRuleSubscriber distributedRuleSubscriber,
            ILogger<Mediator> logger)
        {
            _distributedRuleSubscriber = distributedRuleSubscriber ?? throw new ArgumentNullException(nameof(distributedRuleSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger?.LogInformation("Surveillance.Engine.RuleDistributor.Mediator initiating");
            _distributedRuleSubscriber?.Initiate();
            _logger?.LogInformation("Surveillance.Engine.RuleDistributor.Mediator completed initiating");
        }

        public void Terminate()
        {
            _logger?.LogInformation("Surveillance.Engine.RuleDistributor.Mediator terminating");
            _distributedRuleSubscriber?.Terminate();
            _logger?.LogInformation("Surveillance.Engine.RuleDistributor.Mediator completed terminating");
        }
    }
}
