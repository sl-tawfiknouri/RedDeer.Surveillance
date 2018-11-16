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
            decimal? averagePositionMaximumAbsoluteValueChangeAmount,
            string averagePositionMaximumAbsoluteValueChangeCurrency,
            int? pairingPositionMinimumNumberOfPairedTrades,
            decimal? pairingPositionPercentageValueChangeThresholdPerPair,
            decimal? pairingPositionMaximumAbsoluteCurrencyAmount,
            string pairingPositionMaximumAbsoluteCurrency)
        {
            WindowSize = windowSize;
            AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            AveragePositionMaximumAbsoluteValueChangeAmount = averagePositionMaximumAbsoluteValueChangeAmount;
            AveragePositionMaximumAbsoluteValueChangeCurrency = averagePositionMaximumAbsoluteValueChangeCurrency;

            PairingPositionMinimumNumberOfPairedTrades = pairingPositionMinimumNumberOfPairedTrades;
            PairingPositionPercentageValueChangeThresholdPerPair = pairingPositionPercentageValueChangeThresholdPerPair;
            PairingPositionMaximumAbsoluteCurrencyAmount = pairingPositionMaximumAbsoluteCurrencyAmount;
            PairingPositionMaximumAbsoluteCurrency = pairingPositionMaximumAbsoluteCurrency;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
        }

        public WashTradeRuleParameters(
            TimeSpan windowSize,
            int? averagePositionMinimumNumberOfTrades,
            decimal? averagePositionMaximumPositionValueChange,
            decimal? averagePositionMaximumAbsoluteValueChangeAmount,
            string averagePositionMaximumAbsoluteValueChangeCurrency,
            int? pairingPositionMinimumNumberOfPairedTrades,
            decimal? pairingPositionPercentageValueChangeThresholdPerPair,
            decimal? pairingPositionMaximumAbsoluteCurrencyAmount,
            string pairingPositionMaximumAbsoluteCurrency,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            WindowSize = windowSize;
            AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            AveragePositionMaximumAbsoluteValueChangeAmount = averagePositionMaximumAbsoluteValueChangeAmount;
            AveragePositionMaximumAbsoluteValueChangeCurrency = averagePositionMaximumAbsoluteValueChangeCurrency;

            PairingPositionMinimumNumberOfPairedTrades = pairingPositionMinimumNumberOfPairedTrades;
            PairingPositionPercentageValueChangeThresholdPerPair = pairingPositionPercentageValueChangeThresholdPerPair;
            PairingPositionMaximumAbsoluteCurrencyAmount = pairingPositionMaximumAbsoluteCurrencyAmount;
            PairingPositionMaximumAbsoluteCurrency = pairingPositionMaximumAbsoluteCurrency;

            Accounts = accounts;
            Traders = traders;
            Markets = markets;
        }

        public TimeSpan WindowSize { get; set; }

        // Averaging parameters
        public int? AveragePositionMinimumNumberOfTrades { get; }
        public decimal? AveragePositionMaximumPositionValueChange { get; }
        public decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; }
        public string AveragePositionMaximumAbsoluteValueChangeCurrency { get; }

        // Pairing parameters
        public int? PairingPositionMinimumNumberOfPairedTrades { get; }
        public decimal? PairingPositionPercentageValueChangeThresholdPerPair { get; }
        public decimal? PairingPositionMaximumAbsoluteCurrencyAmount { get; }
        public string PairingPositionMaximumAbsoluteCurrency { get; }

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
