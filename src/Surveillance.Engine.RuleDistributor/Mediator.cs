namespace Surveillance.Engine.RuleDistributor
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.RuleDistributor.Interfaces;
    using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

    public class Mediator : IRuleDistributorMediator
    {
        private readonly IQueueDistributedRuleSubscriber _distributedRuleSubscriber;

        private readonly ILogger<Mediator> _logger;

        public Mediator(IQueueDistributedRuleSubscriber distributedRuleSubscriber, ILogger<Mediator> logger)
        {
            this._distributedRuleSubscriber = distributedRuleSubscriber
                                              ?? throw new ArgumentNullException(nameof(distributedRuleSubscriber));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            this._logger?.LogInformation("initiating");
            this._distributedRuleSubscriber?.Initiate();
            this._logger?.LogInformation("completed initiating");
        }

        public void Terminate()
        {
            this._logger?.LogInformation("terminating");
            this._distributedRuleSubscriber?.Terminate();
            this._logger?.LogInformation("completed terminating");
        }
    }
}