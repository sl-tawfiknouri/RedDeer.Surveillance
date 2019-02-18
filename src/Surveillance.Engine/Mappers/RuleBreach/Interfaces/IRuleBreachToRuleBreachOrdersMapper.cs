using System.Collections.Generic;
using Domain.Trading;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces
{
    public interface IRuleBreachToRuleBreachOrdersMapper
    {
        IReadOnlyCollection<RuleBreachOrder> ProjectToOrders(IRuleBreach ruleBreach, string ruleBreachId);
    }
}