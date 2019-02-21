using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces
{
    public interface IHighVolumeRuleFixedIncomeParameters : IFilterableRule, IRuleParameter
    {
        TimeSpan WindowSize { get; }
    }
}
