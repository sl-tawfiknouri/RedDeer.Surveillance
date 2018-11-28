using System.Collections.Generic;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.Mappers.Interfaces
{
    public interface IClientOrganisationalFactorMapper
    {
        IReadOnlyCollection<ClientOrganisationalFactors> Map(IReadOnlyCollection<OrganisationalFactors> factors);
        ClientOrganisationalFactors Map(OrganisationalFactors factor);
    }
}
