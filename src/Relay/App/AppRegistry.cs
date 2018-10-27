using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Relay.Configuration;
using StructureMap;
using Surveillance.System.DataLayer.Interfaces;

namespace RedDeer.Relay.Relay.App
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<ISystemDataLayerConfig>().Use<Configuration>();
            For<IStartUpTaskRunner>().Use<WebSocketRunner>();
        }
    }
}
