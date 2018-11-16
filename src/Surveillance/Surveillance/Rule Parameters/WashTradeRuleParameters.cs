using System;
using Surveillance.Rule_Parameters.Filter;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class WashTradeRuleParameters : IWashTradeRuleParameters
    {
        public WashTradeRuleParameters(
            TimeSpan windowSize,
            int? averagePositionMinimumNumberOfTrades,
            decimal? averagePositionMaximumPositionValueChange,
            decimal? averagePositionMaximumAbsoluteValueChange,
            int? pairingPositionMinimumNumberOfPairedTrades,
            decimal? pairingPositionPercentageValueChangeThresholdPerPair)
        {
            WindowSize = windowSize;
            AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            AveragePositionMaximumAbsoluteValueChange = averagePositionMaximumAbsoluteValueChange;
            PairingPositionMinimumNumberOfPairedTrades = pairingPositionMinimumNumberOfPairedTrades;
            PairingPositionPercentageValueChangeThresholdPerPair = pairingPositionPercentageValueChangeThresholdPerPair;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
        }

        public WashTradeRuleParameters(
            TimeSpan windowSize,
            int? averagePositionMinimumNumberOfTrades,
            decimal? averagePositionMaximumPositionValueChange,
            decimal? averagePositionMaximumAbsoluteValueChange,
            int? pairingPositionMinimumNumberOfPairedTrades,
            decimal? pairingPositionPercentageValueChangeThresholdPerPair,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            WindowSize = windowSize;
            AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            AveragePositionMaximumAbsoluteValueChange = averagePositionMaximumAbsoluteValueChange;
            PairingPositionMinimumNumberOfPairedTrades = pairingPositionMinimumNumberOfPairedTrades;
            PairingPositionPercentageValueChangeThresholdPerPair = pairingPositionPercentageValueChangeThresholdPerPair;

            Accounts = accounts;
            Traders = traders;
            Markets = markets;
        }

        public TimeSpan WindowSize { get; set; }

        // Averaging parameters
        public int? AveragePositionMinimumNumberOfTrades { get; }
        public decimal? AveragePositionMaximumPositionValueChange { get; }
        public decimal? AveragePositionMaximumAbsoluteValueChange { get; }

        // Pairing parameters
        public int? PairingPositionMinimumNumberOfPairedTrades { get; }
        public decimal? PairingPositionPercentageValueChangeThresholdPerPair { get; }

        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }

        public bool HasFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None;
        }
    }
}
