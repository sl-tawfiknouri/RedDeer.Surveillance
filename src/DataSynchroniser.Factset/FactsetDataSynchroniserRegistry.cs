namespace DataSynchroniser.Api.Factset
{
    using DataSynchroniser.Api.Factset.Factset;
    using DataSynchroniser.Api.Factset.Factset.Interfaces;
    using DataSynchroniser.Api.Factset.Filters;
    using DataSynchroniser.Api.Factset.Filters.Interfaces;
    using DataSynchroniser.Api.Factset.Interfaces;

    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using StructureMap;

    public class FactsetDataSynchroniserRegistry : Registry
    {
        public FactsetDataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IFactsetDataSynchroniser>().Use<FactsetDataSynchroniser>();
            this.For<IFactsetDataRequestFilter>().Use<FactsetDataRequestFilter>();

            this.For<IFactsetDataRequestsManager>().Use<FactsetDataRequestsManager>();
            this.For<IFactsetDataRequestsApiManager>().Use<FactsetDataRequestsApiManager>();
        }
    }
}