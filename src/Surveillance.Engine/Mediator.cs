namespace Surveillance.Engine.Rules
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;

    public class Mediator : IRulesEngineMediator
    {
        private readonly ILogger<Mediator> _logger;

        private readonly IQueueRuleCancellationSubscriber _ruleCancellationSubscriber;

        private readonly IQueueRuleSubscriber _ruleSubscriber;

        public Mediator(
            IQueueRuleSubscriber ruleSubscriber,
            IQueueRuleCancellationSubscriber ruleCancellationSubscriber,
            ILogger<Mediator> logger)
        {
            this._ruleSubscriber = ruleSubscriber ?? throw new ArgumentNullException(nameof(ruleSubscriber));
            this._ruleCancellationSubscriber = ruleCancellationSubscriber
                                               ?? throw new ArgumentNullException(nameof(ruleCancellationSubscriber));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            this._logger?.LogInformation("initiating");
            this._ruleSubscriber.Initiate();
            this._ruleCancellationSubscriber.Initiate();
            this._logger?.LogInformation("initiation complete");
        }

        public void Terminate()
        {
            this._logger?.LogInformation("terminating");
            this._ruleSubscriber.Terminate();
            this._ruleCancellationSubscriber.Terminate();
            this._logger?.LogInformation("termination complete");
        }
    }
}