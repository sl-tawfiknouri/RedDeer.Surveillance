using System;
using Surveillance.Rule_Parameters.Filter;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class HighProfitsRuleParameters : IHighProfitsRuleParameters
    {
        public HighProfitsRuleParameters(
            TimeSpan windowSize,
            decimal? highProfitPercentageThreshold,
            decimal? highProfitAbsoluteThreshold,
            string highProfitAbsoluteThresholdCurrency)
        {
            WindowSize = windowSize;
            HighProfitPercentageThreshold = highProfitPercentageThreshold;
            HighProfitAbsoluteThreshold = highProfitAbsoluteThreshold;
            HighProfitAbsoluteThresholdCurrency = highProfitAbsoluteThresholdCurrency ?? string.Empty;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
        }

        public HighProfitsRuleParameters(
            TimeSpan windowSize,
            decimal? highProfitPercentageThreshold,
            decimal? highProfitAbsoluteThreshold,
            string highProfitAbsoluteThresholdCurrency,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            WindowSize = windowSize;
            HighProfitPercentageThreshold = highProfitPercentageThreshold;
            HighProfitAbsoluteThreshold = highProfitAbsoluteThreshold;
            HighProfitAbsoluteThresholdCurrency = highProfitAbsoluteThresholdCurrency ?? string.Empty;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
        }

        public TimeSpan WindowSize { get; }

        // Percentage
        public decimal? HighProfitPercentageThreshold { get; }

        // Absolute level
        public decimal? HighProfitAbsoluteThreshold { get; }

        /// <summary>
        /// Additional field so we can double check that the currency the absolute threshold was entered in matches the currency
        /// we're trading in. If not - just ignore it for now.
        /// In the future use the exchange rate to figure out whether it exceeded the absolute value or not.
        /// </summary>
        public string HighProfitAbsoluteThresholdCurrency { get; }

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