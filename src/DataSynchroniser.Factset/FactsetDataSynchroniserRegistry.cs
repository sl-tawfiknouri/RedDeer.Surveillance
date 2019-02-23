using DataSynchroniser.Api.Factset.Factset;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using DataSynchroniser.Api.Factset.Filters;
using DataSynchroniser.Api.Factset.Filters.Interfaces;
using DataSynchroniser.Api.Factset.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;

namespace DataSynchroniser.Api.Factset
{
    public class FactsetDataSynchroniserRegistry : Registry
    {
        public FactsetDataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IFactsetDataSynchroniser>().Use<FactsetDataSynchroniser>();
            For<IFactsetDataRequestFilter>().Use<FactsetDataRequestFilter>();

            For<IFactsetDataRequestsManager>().Use<FactsetDataRequestsManager>();
            For<IFactsetDataRequestsApiManager>().Use<FactsetDataRequestsApiManager>();
        }
    }
}
