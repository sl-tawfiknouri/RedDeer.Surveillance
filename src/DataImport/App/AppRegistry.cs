namespace RedDeer.DataImport.DataImport.App
{
    using global::DataImport.Configuration;

    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using StructureMap;

    using Surveillance.Auditing.DataLayer.Interfaces;

    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<ISystemDataLayerConfig>().Use<Configuration>();
            this.For<IStartUpTaskRunner>().Use<DataImportRunner>();
        }
    }
}