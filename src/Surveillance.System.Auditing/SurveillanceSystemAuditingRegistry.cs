using StructureMap;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories;
using Surveillance.System.Auditing.Factories.Interfaces;

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
        }
    }
}
