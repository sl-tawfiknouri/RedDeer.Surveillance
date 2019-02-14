using StructureMap;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Exceptions;
using Surveillance.Auditing.DataLayer.Repositories.Exceptions.Interfaces;
using Surveillance.Auditing.Factories;
using Surveillance.Auditing.Factories.Interfaces;
using Surveillance.Auditing.Logging;
using Surveillance.Auditing.Logging.Interfaces;
using Surveillance.Auditing.Utilities;
using Surveillance.Auditing.Utilities.Interfaces;

namespace Surveillance.Auditing
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
            For<ISystemProcessOperationDataRequestContextFactory>()
                .Use<SystemProcessOperationDataRequestContextFactory>();
            For<IExceptionRepository>().Use<ExceptionRepository>();
            For<IOperationLogging>().Use<OperationLogging>();
            For<IApplicationHeartbeatService>().Use<ApplicationHeartbeatService>();
        }
    }
}