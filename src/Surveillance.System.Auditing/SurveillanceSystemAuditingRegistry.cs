using StructureMap;
using Surveillance.Systems.Auditing.Context;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Systems.Auditing.Factories;
using Surveillance.Systems.Auditing.Factories.Interfaces;
using Surveillance.Systems.Auditing.Logging;
using Surveillance.Systems.Auditing.Logging.Interfaces;
using Surveillance.Systems.Auditing.Utilities;
using Surveillance.Systems.Auditing.Utilities.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Exceptions;
using Surveillance.Systems.DataLayer.Repositories.Exceptions.Interfaces;

namespace Surveillance.Systems.Auditing
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