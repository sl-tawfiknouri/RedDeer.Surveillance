namespace RedDeer.Surveillance.App
{
    using global::Surveillance.Auditing.Context;
    using global::Surveillance.Auditing.DataLayer.Processes;

    using Microsoft.Extensions.Logging;

    using NLog.Extensions.Logging;

    using RedDeer.Surveillance.App.Interfaces;
    using RedDeer.Surveillance.App.ScriptRunner.Interfaces;

    using StructureMap;

    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            SystemProcessContext.ProcessType = SystemProcessType.SurveillanceService;
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IStartUpTaskRunner>().Use<MediatorBootstrapper>();
            this.For<IScriptRunner>().Use<ScriptRunner.ScriptRunner>();
        }
    }
}