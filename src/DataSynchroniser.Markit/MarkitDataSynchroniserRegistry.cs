namespace DataSynchroniser.Api.Markit
{
    using DataSynchroniser.Api.Markit.Interfaces;

    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using StructureMap;

    public class MarkitDataSynchroniserRegistry : Registry
    {
        public MarkitDataSynchroniserRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IMarkitDataSynchroniser>().Use<MarkitDataSynchroniser>();
        }
    }
}