using Surveillance.Rules.Interfaces;

namespace Surveillance.Mappers.RuleBreach.Interfaces
{
    public interface IRuleBreachToRuleBreachMapper
    {
        DomainV2.Trading.RuleBreach RuleBreachItem(IRuleBreach ruleBreach, string description, string caseTitle);
    }
}