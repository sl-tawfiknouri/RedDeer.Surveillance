using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Surveillance.Engine.RuleDistributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Scheduler;
using Surveillance.Engine.RuleDistributor.Scheduler.Interfaces;

namespace Surveillance.Engine.RuleDistributor
{
    public class RuleDistributorRegistry : Registry
    {
        public RuleDistributorRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IRuleDistributorMediator>().Use<Mediator>();
            For<IReddeerDistributedRuleScheduler>().Use<ReddeerDistributedRuleScheduler>();

            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
        }
    }
}
