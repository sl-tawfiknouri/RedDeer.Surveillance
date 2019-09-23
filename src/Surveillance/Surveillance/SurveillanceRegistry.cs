namespace Surveillance
{
    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using StructureMap;

    using Surveillance.Interfaces;

    public class SurveillanceRegistry : Registry
    {
        public SurveillanceRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IMediator>().Use<Mediator>();
        }
    }
}