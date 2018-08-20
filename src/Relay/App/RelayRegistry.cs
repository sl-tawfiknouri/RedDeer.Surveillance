using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using StructureMap;

namespace RedDeer.Relay.App
{
    public class RelayRegistry : Registry
    {
        public RelayRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IStartUpTaskRunner>().Use<NoTaskRunner>();
        }
    }
}
