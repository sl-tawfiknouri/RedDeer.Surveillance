using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces
{
    public interface IRuleBreachToRuleBreachMapper
    {
        Domain.Trading.RuleBreach RuleBreachItem(IRuleBreach ruleBreach, string description, string caseTitle);
    }
}