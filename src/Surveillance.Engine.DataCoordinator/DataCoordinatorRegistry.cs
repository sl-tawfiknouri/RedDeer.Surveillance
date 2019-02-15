using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Surveillance.Engine.DataCoordinator.Coordinator;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

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
            For<IQueueSubscriber>().Use<QueueSubscriber>();
            For<IUploadCoordinator>().Use<UploadCoordinator>();
            For<IScheduleRuleMessageSender>().Use<ScheduleRuleMessageSender>();
            For<IAutoSchedule>().Use<AutoSchedule>();
        }
    }
}
