namespace DataSynchroniser.App
{
    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using StructureMap;

    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IStartUpTaskRunner>().Use<DataSynchroniserRunner>();
        }
    }
}