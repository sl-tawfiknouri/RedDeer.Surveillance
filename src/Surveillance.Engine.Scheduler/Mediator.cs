namespace Surveillance.Engine.Scheduler
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Scheduler.Interfaces;
    using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

    /// <summary>
    /// The mediator.
    /// </summary>
    public class Mediator : IRuleSchedulerMediator
    {
        /// <summary>
        /// The delayed rule scheduler scanner.
        /// </summary>
        private readonly IDelayedSchedulerScanner delayedRuleSchedulerScanner;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<Mediator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="delayedRuleSchedulerScanner">
        /// The delayed rule scheduler scanner.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public Mediator(IDelayedSchedulerScanner delayedRuleSchedulerScanner, ILogger<Mediator> logger)
        {
            this.delayedRuleSchedulerScanner = 
                delayedRuleSchedulerScanner ?? throw new ArgumentNullException(nameof(delayedRuleSchedulerScanner));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The initiate.
        /// </summary>
        public void Initiate()
        {
            this.logger.LogInformation("initiating");
            this.delayedRuleSchedulerScanner.Initiate();
            this.logger.LogInformation("completed initiation");
        }

        /// <summary>
        /// The terminate.
        /// </summary>
        public void Terminate()
        {
            this.logger.LogInformation("terminating");
            this.delayedRuleSchedulerScanner.Terminate();
            this.logger.LogInformation("completed termination");
        }
    }
}