namespace Surveillance.Api.App.Types.Rules
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Api.App.Authorization;

    public class OrganisationTypeEnumGraphType : EnumerationGraphType<OrganisationalFactors>
    {
        public OrganisationTypeEnumGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Name = "organisationFactors";
        }
    }
}