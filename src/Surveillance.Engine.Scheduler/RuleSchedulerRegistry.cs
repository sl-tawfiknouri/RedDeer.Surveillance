namespace Surveillance.Engine.Scheduler
{
    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using StructureMap;

    using Surveillance.Engine.Scheduler.Interfaces;
    using Surveillance.Engine.Scheduler.Queues;
    using Surveillance.Engine.Scheduler.Queues.Interfaces;
    using Surveillance.Engine.Scheduler.Scheduler;
    using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

    /// <summary>
    /// The rule scheduler registry.
    /// </summary>
    public class RuleSchedulerRegistry : Registry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleSchedulerRegistry"/> class.
        /// </summary>
        public RuleSchedulerRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IDelayedScheduler>().Use<DelayedScheduler>();
            this.For<IDelayedSchedulerScanner>().Use<DelayedSchedulerScanner>();
            this.For<IRuleSchedulerMediator>().Use<Mediator>();
            this.For<IQueueScheduledRulePublisher>().Use<QueueScheduledRulePublisher>();
            this.For<IQueueDelayedRuleDistributedPublisher>().Use<QueueDelayedRuleDistributedPublisher>();
        }
    }
}