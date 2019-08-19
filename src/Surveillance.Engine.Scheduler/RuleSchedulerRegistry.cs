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

    public class RuleSchedulerRegistry : Registry
    {
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