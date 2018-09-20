using System;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class CancelledOrderRuleParameters : ICancelledOrderRuleParameters
    {
        public CancelledOrderRuleParameters()
        {
            WindowSize = TimeSpan.FromMinutes(30);
            CancelledOrderPercentagePositionThreshold = 0.8m;
            CancelledOrderCountPercentageThreshold = 0.8m;
            MinimumNumberOfTradesToApplyRuleTo = 3;
            MaximumNumberOfTradesToApplyRuleTo = 10;
        }

        public CancelledOrderRuleParameters(
            TimeSpan windowSize,
            decimal? cancelledOrderPositionPercentageThreshold,
            decimal? cancelledOrderCountPercentageThreshold,
            int minimumNumberOfTradesToApplyRuleTo,
            int? maximumNumberOfTradesToApplyRuleTo)
        {
            WindowSize = windowSize;
            CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;
        }

        public TimeSpan WindowSize { get; }
        public decimal? CancelledOrderPercentagePositionThreshold { get; }
        public decimal? CancelledOrderCountPercentageThreshold { get; }
        public int MinimumNumberOfTradesToApplyRuleTo { get; }
        public int? MaximumNumberOfTradesToApplyRuleTo { get; }
    }
}
