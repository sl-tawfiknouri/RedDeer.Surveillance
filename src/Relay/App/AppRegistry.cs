using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using StructureMap;

namespace RedDeer.Relay.App
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));
            
            For<IStartUpTaskRunner>().Use<WebSocketRunner>();
        }
    }
}
