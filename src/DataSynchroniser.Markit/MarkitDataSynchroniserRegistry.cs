using DataSynchroniser.Api.Markit.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;

namespace DataSynchroniser.Api.Markit
{
    public class MarkitDataSynchroniserRegistry : Registry
    {
        public MarkitDataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMarkitDataSynchroniser>().Use<MarkitDataSynchroniser>();
        }
    }
}
