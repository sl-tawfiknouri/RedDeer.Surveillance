using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces
{
    public interface IOrganisationalFactorBrokerFactory
    {
        IOrganisationalFactorBroker Build(
            IUniverseCloneableRule cloneableRule,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory);
    }
}