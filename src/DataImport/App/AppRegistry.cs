using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;

namespace RedDeer.DataImport.DataImport.App
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IStartUpTaskRunner>().Use<DataImportRunner>();
        }
    }
}