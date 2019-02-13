using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Surveillance.Engine.RuleDistributor.Scheduler;
using Surveillance.Engine.RuleDistributor.Scheduler.Interfaces;
using Surveillance.Engines.Interfaces.Mediator;

namespace Surveillance.Engine.RuleDistributor
{
    public class RuleDistributorRegistry : Registry
    {
        public RuleDistributorRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMediator>().Use<Mediator>();
            For<IReddeerDistributedRuleScheduler>().Use<ReddeerDistributedRuleScheduler>();
        }
    }
}
