namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    using System.Collections.Generic;

    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    public interface IOrganisationalFactorable
    {
        bool AggregateNonFactorableIntoOwnCategory { get; set; }

        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
    }
}