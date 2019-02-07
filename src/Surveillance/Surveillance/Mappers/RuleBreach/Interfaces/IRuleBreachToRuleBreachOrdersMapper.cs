using System.Collections.Generic;
using DomainV2.Trading;
using Surveillance.Rules.Interfaces;

namespace Surveillance.Mappers.RuleBreach.Interfaces
{
    public interface IRuleBreachToRuleBreachOrdersMapper
    {
        IReadOnlyCollection<RuleBreachOrder> ProjectToOrders(IRuleBreach ruleBreach, string ruleBreachId);
    }
}