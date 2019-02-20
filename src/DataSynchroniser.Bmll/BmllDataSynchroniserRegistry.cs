﻿using DataSynchroniser.Api.Bmll.Bmll;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using DataSynchroniser.Api.Bmll.Filters;
using DataSynchroniser.Api.Bmll.Filters.Interfaces;
using DataSynchroniser.Api.Bmll.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using PollyFacade.Policies;
using PollyFacade.Policies.Interfaces;
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
            For<IBmllDataRequestFilter>().Use<BmllDataRequestFilter>();

            For<IBmllDataRequestManager>().Use<BmllDataRequestsManager>();
            For<IBmllDataRequestsApiManager>().Use<BmllDataRequestsApiManager>();
            For<IBmllDataRequestsStorageManager>().Use<BmllDataRequestsStorageManager>();
            For<IGetTimeBarPair>().Use<GetTimeBarPair>();
            For<IMarketDataRequestToMinuteBarRequestKeyDtoProjector>().Use<MarketDataRequestToMinuteBarRequestKeyDtoProjector>();
            For<IBmllDataRequestsGetTimeBars>().Use<BmllDataRequestsGetTimeBars>();
            For<IPolicyFactory>().Use<PolicyFactory>();
        }
    }
}
