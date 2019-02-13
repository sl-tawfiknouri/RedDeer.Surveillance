using System.Collections.Generic;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.Mappers.Interfaces
{
    public interface IClientOrganisationalFactorMapper
    {
        IReadOnlyCollection<ClientOrganisationalFactors> Map(IReadOnlyCollection<OrganisationalFactors> factors);
        ClientOrganisationalFactors Map(OrganisationalFactors factor);
    }
}
