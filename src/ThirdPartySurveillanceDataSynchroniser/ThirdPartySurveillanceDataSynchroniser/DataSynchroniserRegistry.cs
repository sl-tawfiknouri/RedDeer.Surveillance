using DataSynchroniser.Interfaces;
using DataSynchroniser.Manager;
using DataSynchroniser.Manager.Interfaces;
using DataSynchroniser.Queues;
using DataSynchroniser.Queues.Interfaces;
using Domain.Surveillance.Scheduling;
using Domain.Surveillance.Scheduling.Interfaces;
using Infrastructure.Network.Aws_IO;
using Infrastructure.Network.Aws_IO.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SharedKernel.Contracts.Queues;
using SharedKernel.Contracts.Queues.Interfaces;
using StructureMap;

namespace DataSynchroniser
{
    public class DataSynchroniserRegistry : Registry
    {
        public DataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMediator>().Use<Mediator>();

            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IThirdPartyDataRequestSerialiser>().Use<ThirdPartyDataRequestSerialiser>();

            For<IDataRequestSubscriber>().Use<DataRequestSubscriber>();
            For<IDataRequestManager>().Use<DataRequestManager>();
            For<IScheduleRulePublisher>().Use<ScheduleRulePublisher>();

            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
        }
    }
}
