using DataSynchroniser.Api.Refinitive.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Surveillance.Data.Universe.Refinitiv;
using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace DataSynchroniser.Api.Refinitive
{
    public class RefinitivDataSynchroniserRegistry : Registry
    {
        public RefinitivDataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IRefinitivDataSynchroniser>().Use<RefinitivDataSynchroniser>();
            this.For<ITickPriceHistoryServiceClientFactory>().Use<TickPriceHistoryServiceClientFactory>();
        }
    }
}