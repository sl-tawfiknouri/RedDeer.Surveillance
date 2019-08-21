namespace Surveillance.Auditing
{
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

    public class SurveillanceSystemAuditingRegistry : Registry
    {
        public SurveillanceSystemAuditingRegistry()
        {
            this.For<ISystemProcessContext>().Use<SystemProcessContext>().Singleton();
            this.For<ISystemProcessOperationContextFactory>().Use<SystemProcessOperationContextFactory>();
            this.For<ISystemProcessOperationDistributeRuleContextFactory>()
                .Use<SystemProcessOperationDistributeRuleContextFactory>();
            this.For<ISystemProcessOperationRunRuleContextFactory>().Use<SystemProcessOperationRunRuleContextFactory>();
            this.For<ISystemProcessOperationFileUploadContextFactory>()
                .Use<SystemProcessOperationFileUploadContextFactory>();
            this.For<ISystemProcessOperationDataRequestContextFactory>()
                .Use<SystemProcessOperationDataRequestContextFactory>();
            this.For<IExceptionRepository>().Use<ExceptionRepository>();
            this.For<IOperationLogging>().Use<OperationLogging>();
            this.For<IApplicationHeartbeatService>().Use<ApplicationHeartbeatService>();
        }
    }
}