using StructureMap;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Repositories;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer
{
    public class SystemSystemDataLayerRegistry : Registry
    {
        public SystemSystemDataLayerRegistry()
        {
            For<IMigrationRepository>().Use<MigrationRepository>();
            For<IConnectionStringFactory>().Use<ConnectionStringFactory>();

            For<ISystemProcessRepository>().Use<SystemProcessRepository>();
            For<ISystemProcessOperationRepository>().Use<SystemProcessOperationRepository>();
            For<ISystemProcessOperationDistributeRuleRepository>().Use<SystemProcessOperationDistributeRuleRepository>();
            For<ISystemProcessOperationRuleRunRepository>().Use<SystemProcessOperationRuleRunRepository>();
        }
    }
}
