namespace Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Rules;

    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IRuleBreachToRuleBreachOrdersMapper
    {
        IReadOnlyCollection<RuleBreachOrder> ProjectToOrders(IRuleBreach ruleBreach, string ruleBreachId);
    }
}