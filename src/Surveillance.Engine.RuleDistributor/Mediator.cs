namespace Surveillance.Engine.RuleDistributor
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.RuleDistributor.Interfaces;
    using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

    /// <summary>
    /// The mediator.
    /// </summary>
    public class Mediator : IRuleDistributorMediator
    {
        /// <summary>
        /// The distributed rule subscriber.
        /// </summary>
        private readonly IQueueDistributedRuleSubscriber distributedRuleSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<Mediator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="distributedRuleSubscriber">
        /// The distributed rule subscriber.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public Mediator(
            IQueueDistributedRuleSubscriber distributedRuleSubscriber,
            ILogger<Mediator> logger)
        {
            this.distributedRuleSubscriber =
                distributedRuleSubscriber ?? throw new ArgumentNullException(nameof(distributedRuleSubscriber));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The initiate.
        /// </summary>
        public void Initiate()
        {
            this.logger?.LogInformation("initiating");
            this.distributedRuleSubscriber?.Initiate();
            this.logger?.LogInformation("completed initiating");
        }

        /// <summary>
        /// The terminate.
        /// </summary>
        public void Terminate()
        {
            this.logger?.LogInformation("terminating");
            this.distributedRuleSubscriber?.Terminate();
            this.logger?.LogInformation("completed terminating");
        }
    }
}