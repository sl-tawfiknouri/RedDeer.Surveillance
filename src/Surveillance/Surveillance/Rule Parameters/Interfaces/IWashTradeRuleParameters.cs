using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface IWashTradeRuleParameters : IFilterableRule
    {
        TimeSpan WindowSize { get; }

        // Parameter set one
        int? AveragePositionMinimumNumberOfTrades { get; }
        decimal? AveragePositionMaximumPositionValueChange { get; }
        decimal? AveragePositionMaximumAbsoluteValueChange { get; }

        // Parameter set two
        int? PairingPositionMinimumNumberOfPairedTrades { get; }
        decimal? PairingPositionPercentageValueChangeThresholdPerPair { get; }
    }
}
