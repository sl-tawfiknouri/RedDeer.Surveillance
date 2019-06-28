using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    public class HighProfitsRuleEquitiesParameters : IHighProfitsRuleEquitiesParameters
    {
        public HighProfitsRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            bool performHighProfitWindowAnalysis,
            bool performHighProfitDailyAnalysis,
            decimal? highProfitPercentageThreshold,
            decimal? highProfitAbsoluteThreshold,
            bool useCurrencyConversions,
            string highProfitCurrencyConversionTargetCurrency,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            WindowSize = windowSize;
            HighProfitPercentageThreshold = highProfitPercentageThreshold;
            HighProfitAbsoluteThreshold = highProfitAbsoluteThreshold;
            UseCurrencyConversions = useCurrencyConversions;
            HighProfitCurrencyConversionTargetCurrency = highProfitCurrencyConversionTargetCurrency ?? string.Empty;
            PerformHighProfitWindowAnalysis = performHighProfitWindowAnalysis;
            PerformHighProfitDailyAnalysis = performHighProfitDailyAnalysis;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Sectors = RuleFilter.None();
            Industries = RuleFilter.None();
            Regions = RuleFilter.None();
            Countries = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public HighProfitsRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            bool performHighProfitWindowAnalysis,
            bool performHighProfitDailyAnalysis,
            decimal? highProfitPercentageThreshold,
            decimal? highProfitAbsoluteThreshold,
            bool useCurrencyConversions,
            string highProfitCurrencyConversionTargetCurrency,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            WindowSize = windowSize;
            HighProfitPercentageThreshold = highProfitPercentageThreshold;
            HighProfitAbsoluteThreshold = highProfitAbsoluteThreshold;
            UseCurrencyConversions = useCurrencyConversions;
            HighProfitCurrencyConversionTargetCurrency = highProfitCurrencyConversionTargetCurrency ?? string.Empty;
            PerformHighProfitWindowAnalysis = performHighProfitWindowAnalysis;
            PerformHighProfitDailyAnalysis = performHighProfitDailyAnalysis;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategies ?? RuleFilter.None();

            Sectors = sectors ?? RuleFilter.None();
            Industries = industries ?? RuleFilter.None();
            Regions = regions ?? RuleFilter.None();
            Countries = countries ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public string Id { get; }

        public TimeSpan WindowSize { get; }

        public bool PerformHighProfitWindowAnalysis { get; }

        public bool PerformHighProfitDailyAnalysis { get; }

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
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }

        public RuleFilter Sectors { get; set; }
        public RuleFilter Industries { get; set; }
        public RuleFilter Regions { get; set; }
        public RuleFilter Countries { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool HasInternalFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None
                || Funds?.Type != RuleFilterType.None
                || Strategies?.Type != RuleFilterType.None;
        }

        public bool HasReferenceDataFilters()
        {
            return
                Sectors?.Type != RuleFilterType.None
                || Industries?.Type != RuleFilterType.None
                || Regions?.Type != RuleFilterType.None
                || Countries?.Type != RuleFilterType.None;
        }
    }
}