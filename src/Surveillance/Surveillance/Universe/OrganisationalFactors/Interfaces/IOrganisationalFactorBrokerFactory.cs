using System.Collections.Generic;
using Surveillance.RuleParameters.OrganisationalFactors;
using Surveillance.Rules.Interfaces;

namespace Surveillance.Universe.OrganisationalFactors.Interfaces
{
    public interface IOrganisationalFactorBrokerFactory
    {
        IOrganisationalFactorBroker Build(
            IUniverseCloneableRule cloneableRule,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory);
    }
}