namespace Surveillance.Api.App.Types
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;

    public class RulesTypeEnumGraphType : EnumerationGraphType<Domain.Surveillance.Scheduling.Rules>
    {
        public RulesTypeEnumGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Name = "Rules";
        }
    }
}