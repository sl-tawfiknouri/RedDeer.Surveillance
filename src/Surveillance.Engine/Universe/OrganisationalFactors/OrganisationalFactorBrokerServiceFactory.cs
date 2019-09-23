namespace Surveillance.Engine.Rules.Universe.OrganisationalFactors
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;

    public class OrganisationalFactorBrokerServiceFactory : IOrganisationalFactorBrokerServiceFactory
    {
        private readonly ILogger<OrganisationalFactorBrokerService> _logger;

        public OrganisationalFactorBrokerServiceFactory(ILogger<OrganisationalFactorBrokerService> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrganisationalFactorBrokerService Build(
            IUniverseCloneableRule cloneableRule,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            return new OrganisationalFactorBrokerService(
                cloneableRule,
                factors,
                aggregateNonFactorableIntoOwnCategory,
                this._logger);
        }
    }
}