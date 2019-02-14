using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using RedDeer.Surveillance.App.Interfaces;
using RedDeer.Surveillance.App.ScriptRunner.Interfaces;
using StructureMap;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.DataLayer.Processes;

namespace RedDeer.Surveillance.App
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            SystemProcessContext.ProcessType = SystemProcessType.SurveillanceService;
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));
            
            For<IStartUpTaskRunner>().Use<MediatorBootstrapper>();
            For<IScriptRunner>().Use<ScriptRunner.ScriptRunner>();
        }
    }
}
