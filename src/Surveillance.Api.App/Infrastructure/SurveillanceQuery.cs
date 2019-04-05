using Domain.Surveillance.Rules.Interfaces;
using GraphQL.Types;
using Surveillance.Api.App.Types;

namespace Surveillance.Api.App.Infrastructure
{
    public class SurveillanceQuery : ObjectGraphType
    {
        public SurveillanceQuery(IActiveRulesService ruleService)
        {
            Field<ListGraphType<RulesTypeEnumGraphType>>(
                "rules",
                "The category of the rule",
                resolve: context => ruleService.EnabledRules());
        }
    }
}
