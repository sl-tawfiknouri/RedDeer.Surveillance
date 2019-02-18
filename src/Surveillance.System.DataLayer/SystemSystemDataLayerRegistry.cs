﻿using StructureMap;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

namespace Surveillance.Auditing.DataLayer
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
