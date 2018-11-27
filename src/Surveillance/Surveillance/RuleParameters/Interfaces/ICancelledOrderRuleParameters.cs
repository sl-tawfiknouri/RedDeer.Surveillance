using System;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface ICancelledOrderRuleParameters : IFilterableRule
    {
        TimeSpan WindowSize { get; }
        decimal? CancelledOrderPercentagePositionThreshold { get; }
        decimal? CancelledOrderCountPercentageThreshold { get; }
        int MinimumNumberOfTradesToApplyRuleTo { get; }
        int? MaximumNumberOfTradesToApplyRuleTo { get; }
    }
}
