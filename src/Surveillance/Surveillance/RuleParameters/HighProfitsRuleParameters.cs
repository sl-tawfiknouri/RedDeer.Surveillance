using System;
using Surveillance.RuleParameters.Filter;
using Surveillance.RuleParameters.Interfaces;

namespace Surveillance.RuleParameters
{
    public class HighProfitsRuleParameters : IHighProfitsRuleParameters
    {
        public HighProfitsRuleParameters(
            TimeSpan windowSize,
            decimal? highProfitPercentageThreshold,
            decimal? highProfitAbsoluteThreshold,
            bool useCurrencyConversions,
            string highProfitCurrencyConversionTargetCurrency)
        {
            WindowSize = windowSize;
            HighProfitPercentageThreshold = highProfitPercentageThreshold;
            HighProfitAbsoluteThreshold = highProfitAbsoluteThreshold;
            UseCurrencyConversions = useCurrencyConversions;
            HighProfitCurrencyConversionTargetCurrency = highProfitCurrencyConversionTargetCurrency ?? string.Empty;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
        }

        public HighProfitsRuleParameters(
            TimeSpan windowSize,
            decimal? highProfitPercentageThreshold,
            decimal? highProfitAbsoluteThreshold,
            bool useCurrencyConversions,
            string highProfitCurrencyConversionTargetCurrency,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            WindowSize = windowSize;
            HighProfitPercentageThreshold = highProfitPercentageThreshold;
            HighProfitAbsoluteThreshold = highProfitAbsoluteThreshold;
            UseCurrencyConversions = useCurrencyConversions;
            HighProfitCurrencyConversionTargetCurrency = highProfitCurrencyConversionTargetCurrency ?? string.Empty;

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
        /// If true we will use the target currency provided.
        /// Using absolute profits is implicitly always a yes on use currency conversions
        /// </summary>
        public bool UseCurrencyConversions { get; }

        /// <summary>
        /// Target currency if using currency conversions and also used for high profit absolute threshold
        /// </summary>
        public string HighProfitCurrencyConversionTargetCurrency { get; }

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