using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Surveillance.RuleParameters.OrganisationalFactors;
using Surveillance.Rules.Interfaces;
using Surveillance.Universe.OrganisationalFactors.Interfaces;

namespace Surveillance.Universe.OrganisationalFactors
{
    public class OrganisationalFactorBrokerFactory : IOrganisationalFactorBrokerFactory
    {
        private readonly ILogger<OrganisationalFactorBroker> _logger;

        public OrganisationalFactorBrokerFactory(ILogger<OrganisationalFactorBroker> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrganisationalFactorBroker Build(
            IUniverseCloneableRule cloneableRule,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            return new OrganisationalFactorBroker(cloneableRule, factors, aggregateNonFactorableIntoOwnCategory, _logger);
        }
    }
}
