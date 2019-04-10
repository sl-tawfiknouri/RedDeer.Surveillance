using GraphQL.Types;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Api.App.Types.Rules
{
    public class OrganisationTypeEnumGraphType : EnumerationGraphType<OrganisationalFactors>
    {
        public OrganisationTypeEnumGraphType()
        {
            Name = "OrganisationFactors";
        }
    }
}
