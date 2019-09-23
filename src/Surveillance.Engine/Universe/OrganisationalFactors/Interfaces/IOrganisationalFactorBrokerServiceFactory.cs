namespace Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces
{
    using System.Collections.Generic;

    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IOrganisationalFactorBrokerServiceFactory
    {
        IOrganisationalFactorBrokerService Build(
            IUniverseCloneableRule cloneableRule,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory);
    }
}