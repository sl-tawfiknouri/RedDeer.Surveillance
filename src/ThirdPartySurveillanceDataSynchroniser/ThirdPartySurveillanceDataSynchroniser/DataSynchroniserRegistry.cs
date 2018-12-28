using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using ThirdPartySurveillanceDataSynchroniser.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Services;
using ThirdPartySurveillanceDataSynchroniser.Services.Interfaces;

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
            For<IDataRequestsService>().Use<DataRequestsService>();
        }
    }
}
