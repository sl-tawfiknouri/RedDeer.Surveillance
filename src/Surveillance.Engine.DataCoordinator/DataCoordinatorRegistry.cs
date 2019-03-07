using Domain.Surveillance.Rules;
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

namespace Surveillance.Engine.DataCoordinator
{
    public class DataCoordinatorRegistry : Registry
    {
        public DataCoordinatorRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<ICoordinatorMediator>().Use<Mediator>();
            For<IQueueSubscriber>().Use<QueueAutoscheduleSubscriber>();
            For<IQueueScheduleRulePublisher>().Use<QueueScheduleRulePublisher>();
            For<IAutoSchedule>().Use<AutoSchedule>();
            For<IDataVerifier>().Use<DataVerifier>();
            For<IDataCoordinatorScheduler>().Use<DataCoordinatorScheduler>();
            For<ILiveRulesService>().Use<LiveRulesService>();
        }
    }
}
