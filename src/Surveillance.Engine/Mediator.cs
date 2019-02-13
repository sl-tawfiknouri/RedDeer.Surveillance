using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Interfaces;
using Surveillance.Engine.Rules.Scheduler.Interfaces;

namespace Surveillance.Engine.Rules
{
    public class Mediator : IRulesEngineMediator
    {
        private readonly IReddeerRuleScheduler _ruleScheduler;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IReddeerRuleScheduler ruleScheduler,
            ILogger<Mediator> logger)
        {
            _ruleScheduler = ruleScheduler ?? throw new ArgumentNullException(nameof(ruleScheduler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger?.LogInformation("Surveillance.Engine.Rules.Mediator initiating");
            _ruleScheduler.Initiate();
            _logger?.LogInformation("Surveillance.Engine.Rules.Mediator initiation complete");
        }

        public void Terminate()
        {
            _logger?.LogInformation("Surveillance.Engine.Rules.Mediator terminating");
            _ruleScheduler.Terminate();
            _logger?.LogInformation("Surveillance.Engine.Rules.Mediator termination complete");
        }
    }
}