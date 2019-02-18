using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces
{
    public interface IRuleBreachToRuleBreachMapper
    {
        DomainV2.Trading.RuleBreach RuleBreachItem(IRuleBreach ruleBreach, string description, string caseTitle);
    }
}