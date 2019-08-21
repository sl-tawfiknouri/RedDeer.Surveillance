namespace Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces
{
    using Domain.Surveillance.Rules;

    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IRuleBreachToRuleBreachMapper
    {
        RuleBreach RuleBreachItem(IRuleBreach ruleBreach);
    }
}