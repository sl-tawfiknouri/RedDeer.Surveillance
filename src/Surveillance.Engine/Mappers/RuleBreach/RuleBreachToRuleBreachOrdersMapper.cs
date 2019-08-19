namespace Surveillance.Engine.Rules.Mappers.RuleBreach
{
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Rules;

    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public class RuleBreachToRuleBreachOrdersMapper : IRuleBreachToRuleBreachOrdersMapper
    {
        public IReadOnlyCollection<RuleBreachOrder> ProjectToOrders(IRuleBreach ruleBreach, string ruleBreachId)
        {
            if (string.IsNullOrWhiteSpace(ruleBreachId)) return new RuleBreachOrder[0];

            if (ruleBreach == null) return new RuleBreachOrder[0];

            var ruleBreachOrders = ruleBreach.Trades.Get().Where(i => i != null)
                .Select(i => new RuleBreachOrder(ruleBreachId, i.ReddeerOrderId?.ToString())).ToList();

            return ruleBreachOrders;
        }
    }
}