using DomainV2.DTO;
using DomainV2.DTO.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using ThirdPartySurveillanceDataSynchroniser.DataSources;
using ThirdPartySurveillanceDataSynchroniser.DataSources.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Services;
using ThirdPartySurveillanceDataSynchroniser.Services.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser
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
            For<IBmllDataRequestManager>().Use<BmllDataRequestsManager>();
            For<IBmllDataRequestsStorageManager>().Use<BmllDataRequestsStorageManager>();
            For<IBmllDataRequestsRescheduleManager>().Use<BmllDataRequestsRescheduleManager>();
            For<IDataSourceClassifier>().Use<DataSourceClassifier>();
        }
    }
}
