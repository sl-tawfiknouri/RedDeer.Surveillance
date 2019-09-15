namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Tuning;
    using Surveillance.Engine.Rules.RuleParameters.Extensions;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    /// <summary>
    /// The high profits rule fixed income parameters.
    /// </summary>
    [Serializable]
    public class HighProfitsRuleFixedIncomeParameters : IHighProfitsRuleFixedIncomeParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighProfitsRuleFixedIncomeParameters"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="backWindowSize">
        /// The back window size.
        /// </param>
        /// <param name="forwardWindowSize">
        /// The forward window size.
        /// </param>
        /// <param name="performHighProfitWindowAnalysis">
        /// The perform high profit window analysis.
        /// </param>
        /// <param name="performHighProfitDailyAnalysis">
        /// The perform high profit daily analysis.
        /// </param>
        /// <param name="highProfitPercentageThreshold">
        /// The high profit percentage threshold.
        /// </param>
        /// <param name="highProfitAbsoluteThreshold">
        /// The high profit absolute threshold.
        /// </param>
        /// <param name="useCurrencyConversions">
        /// The use currency conversions.
        /// </param>
        /// <param name="highProfitCurrencyConversionTargetCurrency">
        /// The high profit currency conversion target currency.
        /// </param>
        /// <param name="marketCapFilter">
        /// The market cap filter.
        /// </param>
        /// <param name="highVolumeFilter">
        /// The high volume filter.
        /// </param>
        /// <param name="accounts">
        /// The accounts.
        /// </param>
        /// <param name="traders">
        /// The traders.
        /// </param>
        /// <param name="markets">
        /// The markets.
        /// </param>
        /// <param name="funds">
        /// The funds.
        /// </param>
        /// <param name="strategies">
        /// The strategies.
        /// </param>
        /// <param name="sectors">
        /// The sectors.
        /// </param>
        /// <param name="industries">
        /// The industries.
        /// </param>
        /// <param name="regions">
        /// The regions.
        /// </param>
        /// <param name="countries">
        /// The countries.
        /// </param>
        /// <param name="factors">
        /// The factors.
        /// </param>
        /// <param name="aggregateNonFactorableIntoOwnCategory">
        /// The aggregate non factorable into own category.
        /// </param>
        /// <param name="performTuning">
        /// The perform tuning.
        /// </param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HighProfitsRuleFixedIncomeParameters"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="backWindowSize">
        /// The back window size.
        /// </param>
        /// <param name="forwardWindowSize">
        /// The forward window size.
        /// </param>
        /// <param name="performHighProfitWindowAnalysis">
        /// The perform high profit window analysis.
        /// </param>
        /// <param name="performHighProfitDailyAnalysis">
        /// The perform high profit daily analysis.
        /// </param>
        /// <param name="highProfitPercentageThreshold">
        /// The high profit percentage threshold.
        /// </param>
        /// <param name="highProfitAbsoluteThreshold">
        /// The high profit absolute threshold.
        /// </param>
        /// <param name="useCurrencyConversions">
        /// The use currency conversions.
        /// </param>
        /// <param name="highProfitCurrencyConversionTargetCurrency">
        /// The high profit currency conversion target currency.
        /// </param>
        /// <param name="factors">
        /// The factors.
        /// </param>
        /// <param name="aggregateNonFactorableIntoOwnCategory">
        /// The aggregate non factorable into own category.
        /// </param>
        /// <param name="performTuning">
        /// The perform tuning.
        /// </param>
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
        /// Gets or sets the high profit absolute threshold
        /// Not exposed on the user interface at the moment 15/09/2019 - RT
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? HighProfitAbsoluteThreshold { get; set; }

        /// <summary>
        /// Gets the high profit currency conversion target currency.
        /// </summary>
        public string HighProfitCurrencyConversionTargetCurrency { get; }

        /// <summary>
        /// Gets or sets the high profit percentage threshold.
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? HighProfitPercentageThreshold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether perform high profit daily analysis.
        /// </summary>
        public bool PerformHighProfitDailyAnalysis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether perform high profit window analysis.
        /// </summary>
        public bool PerformHighProfitWindowAnalysis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use currency conversions.
        /// </summary>
        public bool UseCurrencyConversions { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [TuneableIdParameter]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether aggregate non factorable into own category.
        /// </summary>
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        /// <summary>
        /// Gets or sets the factors.
        /// </summary>
        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        // FILTERS

        /// <summary>
        /// Gets or sets the filter markets.
        /// </summary>
        public RuleFilter Markets { get; set; }

        /// <summary>
        /// Gets or sets the filter strategies.
        /// </summary>
        public RuleFilter Strategies { get; set; }

        /// <summary>
        /// Gets or sets the filter traders.
        /// </summary>
        public RuleFilter Traders { get; set; }

        /// <summary>
        /// Gets or sets the filter industries.
        /// </summary>
        public RuleFilter Industries { get; set; }

        /// <summary>
        /// Gets or sets the filter funds.
        /// </summary>
        public RuleFilter Funds { get; set; }

        /// <summary>
        /// Gets or sets the filter accounts.
        /// </summary>
        public RuleFilter Accounts { get; set; }

        /// <summary>
        /// Gets or sets the filter countries.
        /// </summary>
        public RuleFilter Countries { get; set; }

        /// <summary>
        /// Gets or sets the filter regions.
        /// </summary>
        public RuleFilter Regions { get; set; }

        /// <summary>
        /// Gets or sets the filter sectors.
        /// </summary>
        public RuleFilter Sectors { get; set; }

        /// <summary>
        /// Gets the market cap filter.
        /// </summary>
        public DecimalRangeRuleFilter MarketCapFilter { get; }

        /// <summary>
        /// Gets or sets the venue volume filter.
        /// </summary>
        public DecimalRangeRuleFilter VenueVolumeFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether perform tuning.
        /// </summary>
        public bool PerformTuning { get; set; }

        /// <summary>
        /// Gets or sets the tuned parameters.
        /// </summary>
        [TunedParam]
        public TunedParameter<string> TunedParameters { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as HighProfitsRuleFixedIncomeParameters;

            if (castObj == null)
            {
                return false;
            }

            return this.Windows == castObj.Windows
                   && this.HighProfitPercentageThreshold == castObj.HighProfitPercentageThreshold
                   && this.HighProfitAbsoluteThreshold == castObj.HighProfitAbsoluteThreshold;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Windows.GetHashCode() * this.HighProfitPercentageThreshold.GetHashCode()
                                              * this.HighProfitAbsoluteThreshold.GetHashCode();
        }

        /// <summary>
        /// The has internal filters.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasInternalFilters()
        {
            return IFilterableRuleExtensions.HasInternalFilters(this);
        }

        /// <summary>
        /// The has market cap filters.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasMarketCapFilters()
        {
            return IMarketCapFilterableExtensions.HasMarketCapFilters(this);
        }

        /// <summary>
        /// The has reference data filters.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasReferenceDataFilters()
        {
            return IReferenceDataFilterableExtensions.HasReferenceDataFilters(this);
        }

        /// <summary>
        /// The has venue volume filters.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasVenueVolumeFilters()
        {
            return IHighVolumeFilterableExtensions.HasVenueVolumeFilters(this);
        }

        /// <summary>
        /// The validity check for parameter settings i.e. no % over 100 etc.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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