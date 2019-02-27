using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces
{
    public interface IWashTradeRuleFixedIncomeParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable
    {
        TimeSpan WindowSize { get; }
    }
}
