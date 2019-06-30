using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    [Serializable]
    public class HighProfitsRuleEquitiesParameters : IHighProfitsRuleEquitiesParameters
    {
        public HighProfitsRuleEquitiesParameters(
            string id,
            TimeSpan backWindowSize,
            TimeSpan forwardWindowSize,
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
            Windows = new TimeWindows(backWindowSize, forwardWindowSize);
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

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public HighProfitsRuleEquitiesParameters(
            string id,
            TimeSpan backWindowSize,
            TimeSpan forwardWindowSize,
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
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;
            Windows = new TimeWindows(backWindowSize, forwardWindowSize);
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

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        [TuneableIdParameter]
        public string Id { get; set; }
        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }
        public bool PerformHighProfitWindowAnalysis { get; set; }

        public bool PerformHighProfitDailyAnalysis { get; set; }

        // Percentage
        [TuneableDecimalParameter]
        public decimal? HighProfitPercentageThreshold { get; set; }

        // Absolute level
        [TuneableDecimalParameter]
        public decimal? HighProfitAbsoluteThreshold { get; set; }

        /// <summary>
        /// If true we will use the target currency provided.
        /// Using absolute profits is implicitly always a yes on use currency conversions
        /// </summary>
        public bool UseCurrencyConversions { get; set; }

        /// <summary>
        /// Target currency if using currency conversions and also used for high profit absolute threshold
        /// </summary>
        public string HighProfitCurrencyConversionTargetCurrency { get; set; }

        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool HasFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None
                || Funds?.Type != RuleFilterType.None
                || Strategies?.Type != RuleFilterType.None;
        }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && (HighProfitPercentageThreshold == null
                    || (HighProfitPercentageThreshold.GetValueOrDefault() >= 0
                        && HighProfitPercentageThreshold.GetValueOrDefault() <= 1))
                && (HighProfitAbsoluteThreshold == null
                    || (HighProfitAbsoluteThreshold.GetValueOrDefault() >= 0));
        }

        public override int GetHashCode()
        {
            return 
                this.Windows.GetHashCode()  
               * this.HighProfitPercentageThreshold.GetHashCode()
               * this.HighProfitAbsoluteThreshold.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as HighProfitsRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return 
                 this.Windows == castObj.Windows          
               && this.HighProfitPercentageThreshold == castObj.HighProfitPercentageThreshold
               && this.HighProfitAbsoluteThreshold == castObj.HighProfitAbsoluteThreshold;
        }

        public bool IsTuned { get; set; }
    }
}