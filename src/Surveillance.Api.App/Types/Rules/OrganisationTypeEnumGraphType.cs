using GraphQL.Authorization;
using GraphQL.Types;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Api.App.Authorization;

namespace Surveillance.Api.App.Types.Rules
{
    public class OrganisationTypeEnumGraphType : EnumerationGraphType<OrganisationalFactors>
    {
        public OrganisationTypeEnumGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Name = "OrganisationFactors";
        }
    }
}
