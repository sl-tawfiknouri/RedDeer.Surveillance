using StructureMap;
using Surveillance.Systems.DataLayer.Interfaces;
using Surveillance.Systems.DataLayer.Repositories;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.DataLayer
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
            For<ISystemProcessOperationUploadFileRepository>().Use<SystemProcessOperationUploadFileRepository>();
            For<ISystemProcessOperationThirdPartyDataRequestRepository>()
                .Use<SystemProcessOperationThirdPartyDataRequestRepository>();
        }
    }
}
