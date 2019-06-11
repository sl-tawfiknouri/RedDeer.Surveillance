using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Surveillance.Engine.Scheduler.Interfaces;
using Surveillance.Engine.Scheduler.Queues;
using Surveillance.Engine.Scheduler.Queues.Interfaces;

namespace Surveillance.Engine.Scheduler
{
    public class RuleSchedulerRegistry : Registry
    {
        public RuleSchedulerRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IRuleSchedulerMediator>().Use<Mediator>();
            For<IQueueDelayedRuleSchedulerSubscriber>().Use<QueueDelayedRuleSchedulerSubscriber>();
        }
    }
}
