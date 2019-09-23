namespace Surveillance.Engine.Scheduler
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Scheduler.Interfaces;
    using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

    public class Mediator : IRuleSchedulerMediator
    {
        private readonly IDelayedSchedulerScanner _delayedRuleSchedulerScanner;

        private readonly ILogger<Mediator> _logger;

        public Mediator(IDelayedSchedulerScanner delayedRuleSchedulerScanner, ILogger<Mediator> logger)
        {
            this._delayedRuleSchedulerScanner = delayedRuleSchedulerScanner
                                                ?? throw new ArgumentNullException(nameof(delayedRuleSchedulerScanner));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            this._logger.LogInformation("initiating");
            this._delayedRuleSchedulerScanner.Initiate();
            this._logger.LogInformation("completed initiation");
        }

        public void Terminate()
        {
            this._logger.LogInformation("terminating");
            this._delayedRuleSchedulerScanner.Terminate();
            this._logger.LogInformation("completed termination");
        }
    }
}