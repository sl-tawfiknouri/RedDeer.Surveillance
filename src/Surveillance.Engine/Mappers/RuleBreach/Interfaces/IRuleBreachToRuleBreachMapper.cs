using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces
{
    public interface IRuleBreachToRuleBreachMapper
    {
        Domain.Surveillance.Rules.RuleBreach RuleBreachItem(IRuleBreach ruleBreach);
    }
}