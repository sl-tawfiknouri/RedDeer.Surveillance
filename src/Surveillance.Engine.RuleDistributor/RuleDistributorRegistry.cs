namespace Surveillance.Engine.RuleDistributor
{
    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Scheduling.Interfaces;

    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using StructureMap;

    using Surveillance.Engine.RuleDistributor.Distributor;
    using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
    using Surveillance.Engine.RuleDistributor.Interfaces;
    using Surveillance.Engine.RuleDistributor.Queues;
    using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

    /// <summary>
    /// The rule distributor registry.
    /// </summary>
    public class RuleDistributorRegistry : Registry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDistributorRegistry"/> class.
        /// </summary>
        public RuleDistributorRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IRuleDistributorMediator>().Use<Mediator>();
            this.For<IQueueDistributedRuleSubscriber>().Use<QueueDistributedRuleSubscriber>();

            this.For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            this.For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
            this.For<IScheduleDisassembler>().Use<ScheduleDisassembler>();
            this.For<IQueueDistributedRulePublisher>().Use<QueueDistributedRulePublisher>();
        }
    }
}