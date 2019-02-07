using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;

namespace RedDeer.ThirdPartySurveillanceDataSynchroniser.App
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IStartUpTaskRunner>().Use<DataSynchroniserRunner>();
        }
    }
}
