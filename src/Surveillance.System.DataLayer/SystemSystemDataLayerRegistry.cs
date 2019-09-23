namespace Surveillance.Auditing.DataLayer
{
    using StructureMap;

    using Surveillance.Auditing.DataLayer.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

    public class SystemSystemDataLayerRegistry : Registry
    {
        public SystemSystemDataLayerRegistry()
        {
            this.For<IMigrationRepository>().Use<MigrationRepository>();
            this.For<IConnectionStringFactory>().Use<ConnectionStringFactory>();

            this.For<ISystemProcessRepository>().Use<SystemProcessRepository>();
            this.For<ISystemProcessOperationRepository>().Use<SystemProcessOperationRepository>();
            this.For<ISystemProcessOperationDistributeRuleRepository>()
                .Use<SystemProcessOperationDistributeRuleRepository>();
            this.For<ISystemProcessOperationRuleRunRepository>().Use<SystemProcessOperationRuleRunRepository>();
            this.For<ISystemProcessOperationUploadFileRepository>().Use<SystemProcessOperationUploadFileRepository>();
            this.For<ISystemProcessOperationThirdPartyDataRequestRepository>()
                .Use<SystemProcessOperationThirdPartyDataRequestRepository>();
        }
    }
}