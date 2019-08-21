namespace DataSynchroniser.Api.Bmll
{
    using DataSynchroniser.Api.Bmll.Bmll;
    using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
    using DataSynchroniser.Api.Bmll.Filters;
    using DataSynchroniser.Api.Bmll.Filters.Interfaces;
    using DataSynchroniser.Api.Bmll.Interfaces;

    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using PollyFacade.Policies;
    using PollyFacade.Policies.Interfaces;

    using StructureMap;

    public class BmllDataSynchroniserRegistry : Registry
    {
        public BmllDataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IBmllDataSynchroniser>().Use<BmllDataSynchroniser>();
            this.For<IBmllDataRequestFilter>().Use<BmllDataRequestFilter>();

            this.For<IBmllDataRequestManager>().Use<BmllDataRequestsManager>();
            this.For<IBmllDataRequestsApiManager>().Use<BmllDataRequestsApiManager>();
            this.For<IBmllDataRequestsStorageManager>().Use<BmllDataRequestsStorageManager>();
            this.For<IGetTimeBarPair>().Use<GetTimeBarPair>();
            this.For<IMarketDataRequestToMinuteBarRequestKeyDtoProjector>()
                .Use<MarketDataRequestToMinuteBarRequestKeyDtoProjector>();
            this.For<IBmllDataRequestsGetTimeBars>().Use<BmllDataRequestsGetTimeBars>();
            this.For<IPolicyFactory>().Use<PolicyFactory>();
        }
    }
}