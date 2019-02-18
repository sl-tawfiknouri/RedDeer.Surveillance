using DataSynchroniser.DataSources;
using DataSynchroniser.DataSources.Interfaces;
using DataSynchroniser.Interfaces;
using DataSynchroniser.Manager;
using DataSynchroniser.Manager.Bmll;
using DataSynchroniser.Manager.Bmll.Interfaces;
using DataSynchroniser.Manager.Factset;
using DataSynchroniser.Manager.Factset.Interfaces;
using DataSynchroniser.Manager.Interfaces;
using DataSynchroniser.Services;
using DataSynchroniser.Services.Interfaces;
using Domain.DTO;
using Domain.DTO.Interfaces;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

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

            For<IDataRequestsService>().Use<DataRequestsService>();
            For<IDataRequestManager>().Use<DataRequestManager>();
            For<IDataSourceClassifier>().Use<DataSourceClassifier>();

            For<IBmllDataRequestManager>().Use<BmllDataRequestsManager>();
            For<IBmllDataRequestsSenderManager>().Use<BmllDataRequestsSenderManager>();
            For<IBmllDataRequestsStorageManager>().Use<BmllDataRequestsStorageManager>();
            For<IBmllDataRequestsRescheduleManager>().Use<BmllDataRequestsRescheduleManager>();

            For<IFactsetDataRequestsManager>().Use<FactsetDataRequestsManager>();
            For<IFactsetDataRequestsSenderManager>().Use<FactsetDataRequestsSenderManager>();
            For<IFactsetDataRequestsStorageManager>().Use<FactsetDataRequestsStorageManager>();

            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
        }
    }
}
