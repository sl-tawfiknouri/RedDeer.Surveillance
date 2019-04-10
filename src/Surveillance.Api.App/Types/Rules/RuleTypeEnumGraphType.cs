using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;

namespace Surveillance.Api.App.Types
{
    public class RulesTypeEnumGraphType : EnumerationGraphType<Domain.Surveillance.Scheduling.Rules>
    {
        public RulesTypeEnumGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            Name = "Rules";
        }
    }
}
