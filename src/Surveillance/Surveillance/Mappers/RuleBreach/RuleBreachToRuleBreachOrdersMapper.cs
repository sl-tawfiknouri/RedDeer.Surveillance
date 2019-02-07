using System.Collections.Generic;
using System.Linq;
using DomainV2.Trading;
using Surveillance.Mappers.RuleBreach.Interfaces;
using Surveillance.Rules.Interfaces;

namespace Surveillance.Mappers.RuleBreach
{
    public class RuleBreachToRuleBreachOrdersMapper : IRuleBreachToRuleBreachOrdersMapper
    {
        public IReadOnlyCollection<RuleBreachOrder> ProjectToOrders(IRuleBreach ruleBreach, string ruleBreachId)
        {
            if (string.IsNullOrWhiteSpace(ruleBreachId))
            {
                return new RuleBreachOrder[0];
            }

            if (ruleBreach == null)
            {
                return new RuleBreachOrder[0];
            }

            var ruleBreachOrders = ruleBreach
                .Trades
                .Get()
                .Where(i => i != null)
                .Select(i => new RuleBreachOrder(ruleBreachId, i.ReddeerOrderId?.ToString()))
                .ToList();

            return ruleBreachOrders;
        }
    }
}
