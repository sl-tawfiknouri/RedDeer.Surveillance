using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface ICancelledOrderRuleParameters
    {
        TimeSpan WindowSize { get; }
        decimal? CancelledOrderPercentagePositionThreshold { get; }
        decimal? CancelledOrderCountPercentageThreshold { get; }
        int MinimumNumberOfTradesToApplyRuleTo { get; }
        int MaximumNumberOfTradesToApplyRuleTo { get; }
    }
}
