using StructureMap;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.Auditing.Logging;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.Auditing.Utilities;
using Surveillance.System.Auditing.Utilities.Interfaces;
using Surveillance.System.DataLayer.Repositories.Exceptions;
using Surveillance.System.DataLayer.Repositories.Exceptions.Interfaces;

namespace Surveillance.System.Auditing
{
    public class SurveillanceSystemAuditingRegistry : Registry
    {
        public SurveillanceSystemAuditingRegistry()
        {
            For<ISystemProcessContext>().Use<SystemProcessContext>().Singleton();
            For<ISystemProcessOperationContextFactory>().Use<SystemProcessOperationContextFactory>();
            For<ISystemProcessOperationDistributeRuleContextFactory>().Use<SystemProcessOperationDistributeRuleContextFactory>();
            For<ISystemProcessOperationRunRuleContextFactory>().Use<SystemProcessOperationRunRuleContextFactory>();
            For<ISystemProcessOperationFileUploadContextFactory>().Use<SystemProcessOperationFileUploadContextFactory>();
            For<IExceptionRepository>().Use<ExceptionRepository>();
            For<IOperationLogging>().Use<OperationLogging>();
            For<IApplicationHeartbeatService>().Use<ApplicationHeartbeatService>();
        }
    }
}