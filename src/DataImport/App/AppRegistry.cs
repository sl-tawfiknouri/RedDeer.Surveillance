using DataImport.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.System.DataLayer.Interfaces;

namespace RedDeer.DataImport.DataImport.App
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<ISystemDataLayerConfig>().Use<Configuration>();
            //For<IDataLayerConfiguration>().Use<DataLayerConfiguration>();
            For<IStartUpTaskRunner>().Use<WebSocketRunner>();
        }
    }
}
