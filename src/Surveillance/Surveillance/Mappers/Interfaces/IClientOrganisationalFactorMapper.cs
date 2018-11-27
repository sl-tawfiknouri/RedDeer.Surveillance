using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.Mappers.Interfaces
{
    public interface IClientOrganisationalFactorMapper
    {
        ClientOrganisationalFactors Map(OrganisationalFactors factor);
    }
}
