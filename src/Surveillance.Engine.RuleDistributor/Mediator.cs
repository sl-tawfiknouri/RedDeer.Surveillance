using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.RuleDistributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Scheduler.Interfaces;

namespace Surveillance.Engine.RuleDistributor
{
    public class Mediator : IRuleDistributorMediator
    {
        private readonly IReddeerDistributedRuleScheduler _distributedRuleScheduler;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IReddeerDistributedRuleScheduler distributedRuleScheduler,
            ILogger<Mediator> logger)
        {
            _distributedRuleScheduler = distributedRuleScheduler ?? throw new ArgumentNullException(nameof(distributedRuleScheduler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger?.LogInformation("Surveillance.Engine.RuleDistributor.Mediator initiating");
            _distributedRuleScheduler?.Initiate();
            _logger?.LogInformation("Surveillance.Engine.RuleDistributor.Mediator completed initiating");
        }

        public void Terminate()
        {
            _logger?.LogInformation("Surveillance.Engine.RuleDistributor.Mediator terminating");
            _distributedRuleScheduler?.Terminate();
            _logger?.LogInformation("Surveillance.Engine.RuleDistributor.Mediator completed terminating");
        }
    }
}
