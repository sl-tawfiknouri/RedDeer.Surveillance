using DataSynchroniser.Api.Bmll.Bmll;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using DataSynchroniser.Api.Bmll.Filters;
using DataSynchroniser.Api.Bmll.Filters.Interfaces;
using DataSynchroniser.Api.Bmll.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;

namespace DataSynchroniser.Api.Bmll
{
    public class BmllDataSynchroniserRegistry : Registry
    {
        public BmllDataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IBmllDataSynchroniser>().Use<BmllDataSynchroniser>();
            For<IMarketDataRequestFilter>().Use<MarketDataRequestFilter>();

            For<IBmllDataRequestManager>().Use<BmllDataRequestsManager>();
            For<IBmllDataRequestsSenderManager>().Use<BmllDataRequestsSenderManager>();
            For<IBmllDataRequestsStorageManager>().Use<BmllDataRequestsStorageManager>();
            For<IGetTimeBarPair>().Use<GetTimeBarPair>();
        }
    }
}
