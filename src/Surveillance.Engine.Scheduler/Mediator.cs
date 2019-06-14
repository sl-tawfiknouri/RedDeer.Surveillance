using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Scheduler.Interfaces;
using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

namespace Surveillance.Engine.Scheduler
{
    public class Mediator : IRuleSchedulerMediator
    {
        private readonly IDelayedSchedulerScanner _delayedRuleSchedulerScanner;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IDelayedSchedulerScanner delayedRuleSchedulerScanner,
            ILogger<Mediator> logger)
        {
            _delayedRuleSchedulerScanner = delayedRuleSchedulerScanner ?? throw new ArgumentNullException(nameof(delayedRuleSchedulerScanner));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"initiating");
            _delayedRuleSchedulerScanner.Initiate();
            _logger.LogInformation($"completed initiation");
        }

        public void Terminate()
        {
            _logger.LogInformation($"terminating");
            _delayedRuleSchedulerScanner.Terminate();
            _logger.LogInformation($"completed termination");
        }
    }
}
