using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Surveillance.Engine.DataCoordinator.Interfaces;

namespace Surveillance.Engine.DataCoordinator
{
    public class DataCoordinatorRegistry : Registry
    {
        public DataCoordinatorRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<ICoordinatorMediator>().Use<Mediator>();
        }
    }
}
