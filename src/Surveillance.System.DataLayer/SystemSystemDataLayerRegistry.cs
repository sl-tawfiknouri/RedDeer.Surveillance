using StructureMap;
using Surveillance.System.DataLayer.Repositories;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer
{
    public class SystemSystemDataLayerRegistry : Registry
    {
        public SystemSystemDataLayerRegistry()
        {
            For<ISystemProcessRepository>().Use<SystemProcessRepository>();
            For<ISystemProcessOperationRepository>().Use<SystemProcessOperationRepository>();
            For<ISystemProcessOperationDistributeRuleRepository>().Use<SystemProcessOperationDistributeRuleRepository>();
            For<ISystemProcessOperationFileRepository>().Use<SystemProcessOperationFileRepository>();
            For<ISystemProcessOperationRuleRunRepository>().Use<SystemProcessOperationRuleRunRepository>();
        }
    }
}
