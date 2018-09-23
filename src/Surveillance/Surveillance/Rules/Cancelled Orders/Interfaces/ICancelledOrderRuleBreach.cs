﻿using Surveillance.Rules.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders.Interfaces
{
    public interface ICancelledOrderRuleBreach : IRuleBreach
    {
        ICancelledOrderRuleParameters Parameters { get; }

        bool HasBreachedRule();
        bool ExceededPercentagePositionCancellations { get; }
        decimal? PercentagePositionCancelled { get; }
        bool ExceededPercentageTradeCountCancellations { get; }
        decimal? PercentageTradeCountCancelled { get; }
    }
}