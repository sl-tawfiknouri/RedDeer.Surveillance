namespace DataSynchroniser
{
    using DataSynchroniser.Interfaces;
    using DataSynchroniser.Manager;
    using DataSynchroniser.Manager.Interfaces;
    using DataSynchroniser.Queues;
    using DataSynchroniser.Queues.Interfaces;

    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Scheduling.Interfaces;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using SharedKernel.Contracts.Queues;
    using SharedKernel.Contracts.Queues.Interfaces;

    using StructureMap;

    public class DataSynchroniserRegistry : Registry
    {
        public DataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IMediator>().Use<Mediator>();

            this.For<IAwsQueueClient>().Use<AwsQueueClient>();
            this.For<IThirdPartyDataRequestSerialiser>().Use<ThirdPartyDataRequestSerialiser>();

            this.For<IDataRequestSubscriber>().Use<DataRequestSubscriber>();
            this.For<IDataRequestManager>().Use<DataRequestManager>();
            this.For<IScheduleRulePublisher>().Use<ScheduleRulePublisher>();

            this.For<IAwsQueueClient>().Use<AwsQueueClient>();
            this.For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            this.For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
        }
    }
}