using StructureMap;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.System.Auditing
{
    public class SurveillanceSystemAuditingRegistry : Registry
    {
        public SurveillanceSystemAuditingRegistry()
        {
            For<ISystemProcessContext>().Use<SystemProcessContext>();
        }
    }
}
