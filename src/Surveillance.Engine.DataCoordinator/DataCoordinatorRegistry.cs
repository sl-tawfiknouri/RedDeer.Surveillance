namespace Surveillance.Engine.DataCoordinator
{
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;

    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using StructureMap;

    using Surveillance.Engine.DataCoordinator.Coordinator;
    using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Queues;
    using Surveillance.Engine.DataCoordinator.Queues.Interfaces;
    using Surveillance.Engine.DataCoordinator.Scheduler;
    using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

    /// <summary>
    /// The data coordinator registry.
    /// </summary>
    public class DataCoordinatorRegistry : Registry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataCoordinatorRegistry"/> class.
        /// </summary>
        public DataCoordinatorRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<ICoordinatorMediator>().Use<Mediator>();
            this.For<IQueueSubscriber>().Use<QueueAutoScheduleSubscriber>();
            this.For<IQueueScheduleRulePublisher>().Use<QueueScheduleRulePublisher>();
            this.For<IAutoSchedule>().Use<AutoSchedule>();
            this.For<IDataVerifier>().Use<DataVerifier>();
            this.For<IDataCoordinatorScheduler>().Use<DataCoordinatorScheduler>();
            this.For<IActiveRulesService>().Use<ActiveRulesService>();
        }
    }
}