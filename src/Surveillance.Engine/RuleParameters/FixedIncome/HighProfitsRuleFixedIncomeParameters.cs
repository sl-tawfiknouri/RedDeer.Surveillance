namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Tuning;
    using Surveillance.Engine.Rules.RuleParameters.Extensions;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    [Serializable]
    public class HighProfitsRuleFixedIncomeParameters : IHighProfitsRuleFixedIncomeParameters
    {
        public HighProfitsRuleFixedIncomeParameters(
            string id,
            TimeSpan backWindowSize,
            TimeSpan forwardWindowSize,
            bool performHighProfitWindowAnalysis,
            bool performHighProfitDailyAnalysis,
            decimal? highProfitPercentageThreshold,
            decimal? highProfitAbsoluteThreshold,
            bool useCurrencyConversions,
            string highProfitCurrencyConversionTargetCurrency,
            DecimalRangeRuleFilter marketCapFilter,
            DecimalRangeRuleFilter highVolumeFilter,
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
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            this.Id = id ?? string.Empty;
            this.Windows = new TimeWindows(id, backWindowSize, forwardWindowSize);
            this.HighProfitPercentageThreshold = highProfitPercentageThreshold;
            this.HighProfitAbsoluteThreshold = highProfitAbsoluteThreshold;
            this.UseCurrencyConversions = useCurrencyConversions;
            this.HighProfitCurrencyConversionTargetCurrency =
                highProfitCurrencyConversionTargetCurrency ?? string.Empty;
            this.PerformHighProfitWindowAnalysis = performHighProfitWindowAnalysis;
            this.PerformHighProfitDailyAnalysis = performHighProfitDailyAnalysis;

            this.MarketCapFilter = marketCapFilter ?? DecimalRangeRuleFilter.None();
            this.VenueVolumeFilter = highVolumeFilter ?? DecimalRangeRuleFilter.None();

            this.Accounts = accounts ?? RuleFilter.None();
            this.Traders = traders ?? RuleFilter.None();
            this.Markets = markets ?? RuleFilter.None();
            this.Funds = funds ?? RuleFilter.None();
            this.Strategies = strategies ?? RuleFilter.None();

            this.Sectors = sectors ?? RuleFilter.None();
            this.Industries = industries ?? RuleFilter.None();
            this.Regions = regions ?? RuleFilter.None();
            this.Countries = countries ?? RuleFilter.None();

            this.Factors = factors ?? new ClientOrganisationalFactors[0];
            this.AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            this.PerformTuning = performTuning;
        }

        public HighProfitsRuleFixedIncomeParameters(
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
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            this.Id = id ?? string.Empty;
            this.Windows = new TimeWindows(id, backWindowSize, forwardWindowSize);
            this.HighProfitPercentageThreshold = highProfitPercentageThreshold;
            this.HighProfitAbsoluteThreshold = highProfitAbsoluteThreshold;
            this.UseCurrencyConversions = useCurrencyConversions;
            this.HighProfitCurrencyConversionTargetCurrency =
                highProfitCurrencyConversionTargetCurrency ?? string.Empty;
            this.PerformHighProfitWindowAnalysis = performHighProfitWindowAnalysis;
            this.PerformHighProfitDailyAnalysis = performHighProfitDailyAnalysis;

            this.MarketCapFilter = DecimalRangeRuleFilter.None();
            this.VenueVolumeFilter = DecimalRangeRuleFilter.None();

            this.Accounts = RuleFilter.None();
            this.Traders = RuleFilter.None();
            this.Markets = RuleFilter.None();
            this.Funds = RuleFilter.None();
            this.Strategies = RuleFilter.None();

            this.Sectors = RuleFilter.None();
            this.Industries = RuleFilter.None();
            this.Regions = RuleFilter.None();
            this.Countries = RuleFilter.None();

            this.Factors = factors ?? new ClientOrganisationalFactors[0];
            this.AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            this.PerformTuning = performTuning;
        }

        /// <summary>
        /// HIGH PROFIT RULE SETTINGS
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? HighProfitAbsoluteThreshold { get; set; }

        /// <summary>
        ///     Target currency if using currency conversions and also used for high profit absolute threshold
        /// </summary>
        public string HighProfitCurrencyConversionTargetCurrency { get; }

        // Percentage
        [TuneableDecimalParameter]
        public decimal? HighProfitPercentageThreshold { get; set; }

        public bool PerformHighProfitDailyAnalysis { get; set; }

        public bool PerformHighProfitWindowAnalysis { get; set; }

        public bool UseCurrencyConversions { get; set; }

        /// <summary>
        /// IDENTIFIER
        /// </summary>
        [TuneableIdParameter]
        public string Id { get; set; }

        /// <summary>
        /// ORGANISATIONAL FACTORING
        /// </summary>
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        /// <summary>
        /// FILTERS
        /// </summary>
        public RuleFilter Markets { get; set; }

        public RuleFilter Strategies { get; set; }

        public RuleFilter Traders { get; set; }

        public RuleFilter Industries { get; set; }

        public RuleFilter Funds { get; set; }

        public RuleFilter Accounts { get; set; }

        public RuleFilter Countries { get; set; }
        public RuleFilter Regions { get; set; }
        public RuleFilter Sectors { get; set; }

        public DecimalRangeRuleFilter MarketCapFilter { get; }

        public DecimalRangeRuleFilter VenueVolumeFilter { get; set; }

        /// <summary>
        /// TUNING
        /// </summary>
        public bool PerformTuning { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }

        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var castObj = obj as HighProfitsRuleFixedIncomeParameters;

            if (castObj == null) return false;

            return this.Windows == castObj.Windows
                   && this.HighProfitPercentageThreshold == castObj.HighProfitPercentageThreshold
                   && this.HighProfitAbsoluteThreshold == castObj.HighProfitAbsoluteThreshold;
        }

        public override int GetHashCode()
        {
            return this.Windows.GetHashCode() * this.HighProfitPercentageThreshold.GetHashCode()
                                              * this.HighProfitAbsoluteThreshold.GetHashCode();
        }

        public bool HasInternalFilters()
        {
            return IFilterableRuleExtensions.HasInternalFilters(this);
        }

        public bool HasMarketCapFilters()
        {
            return IMarketCapFilterableExtensions.HasMarketCapFilters(this);
        }

        public bool HasReferenceDataFilters()
        {
            return IReferenceDataFilterableExtensions.HasReferenceDataFilters(this);
        }

        public bool HasVenueVolumeFilters()
        {
            return IHighVolumeFilterableExtensions.HasVenueVolumeFilters(this);
        }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(this.Id)
                   && (this.HighProfitPercentageThreshold == null
                       || this.HighProfitPercentageThreshold.GetValueOrDefault() >= 0
                       && this.HighProfitPercentageThreshold.GetValueOrDefault() <= 1)
                   && (this.HighProfitAbsoluteThreshold == null
                       || this.HighProfitAbsoluteThreshold.GetValueOrDefault() >= 0);
        }
    }
}