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
            For<ISystemProcessContext>().Use<SystemProcessContext>();
            For<ISystemProcessOperationContextFactory>().Use<SystemProcessOperationContextFactory>();
        }
    }
}
