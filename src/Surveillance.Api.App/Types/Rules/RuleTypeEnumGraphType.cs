using GraphQL.Types;

namespace Surveillance.Api.App.Types
{
    public class RulesTypeEnumGraphType : EnumerationGraphType<Domain.Surveillance.Scheduling.Rules>
    {
        public RulesTypeEnumGraphType()
        {
            Name = "Rules";
        }
    }
}
