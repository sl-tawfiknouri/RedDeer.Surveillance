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
    /// The high volume issuance rule fixed income parameters.
    /// </summary>
    [Serializable]
    public class HighVolumeIssuanceRuleFixedIncomeParameters : IHighVolumeIssuanceRuleFixedIncomeParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighVolumeIssuanceRuleFixedIncomeParameters"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="windowSize">
        /// The window size.
        /// </param>
        /// <param name="percentageDaily">
        /// The daily percentage threshold
        /// </param>
        /// <param name="percentageWindow">
        /// The window percentage threshold
        /// &gt;
        /// </param>
        /// <param name="marketCapFilter">
        /// The market cap filter
        /// </param>
        /// <param name="highVolumeFilter">
        /// The venue volume filter.
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
        /// The industry sectors
        /// </param>
        /// <param name="industries">
        /// The industries
        /// </param>
        /// <param name="regions">
        /// The regions
        /// </param>
        /// <param name="countries">
        /// The countries filter
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
        public HighVolumeIssuanceRuleFixedIncomeParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageDaily,
            decimal? percentageWindow,
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
            this.Windows = new TimeWindows(id, windowSize);
            this.Accounts = accounts ?? RuleFilter.None();
            this.Traders = traders ?? RuleFilter.None();
            this.Markets = markets ?? RuleFilter.None();
            this.Funds = funds ?? RuleFilter.None();
            this.Strategies = strategies ?? RuleFilter.None();

            this.Factors = factors ?? new List<ClientOrganisationalFactors>();
            this.AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            this.FixedIncomeHighVolumePercentageDaily = percentageDaily;
            this.FixedIncomeHighVolumePercentageWindow = percentageWindow;
            this.MarketCapFilter = marketCapFilter ?? DecimalRangeRuleFilter.None();
            this.VenueVolumeFilter = highVolumeFilter ?? DecimalRangeRuleFilter.None();
            this.Sectors = sectors ?? RuleFilter.None();
            this.Industries = industries ?? RuleFilter.None();
            this.Regions = regions ?? RuleFilter.None();
            this.Countries = countries ?? RuleFilter.None();

            this.PerformTuning = performTuning;
        }

        /// <summary>
        /// Gets or sets the fixed income high volume percentage daily.
        /// </summary>
        [TuneableIdParameter]
        public decimal? FixedIncomeHighVolumePercentageDaily { get; set; }

        /// <summary>
        /// Gets or sets the fixed income high volume percentage window.
        /// </summary>
        [TuneableIdParameter]
        public decimal? FixedIncomeHighVolumePercentageWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether aggregate non factorable into own category.
        /// </summary>
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        /// <summary>
        /// Gets or sets the factors.
        /// </summary>
        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [TuneableIdParameter]
        public string Id { get; set; }

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
        /// Gets or sets the accounts.
        /// </summary>
        public RuleFilter Accounts { get; set; }

        /// <summary>
        /// Gets or sets the funds.
        /// </summary>
        public RuleFilter Funds { get; set; }

        /// <summary>
        /// Gets or sets the markets.
        /// </summary>
        public RuleFilter Markets { get; set; }

        /// <summary>
        /// Gets or sets the strategies.
        /// </summary>
        public RuleFilter Strategies { get; set; }

        /// <summary>
        /// Gets or sets the traders.
        /// </summary>
        public RuleFilter Traders { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        public RuleFilter Countries { get; set; }

        /// <summary>
        /// Gets or sets the industries.
        /// </summary>
        public RuleFilter Industries { get; set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        public RuleFilter Regions { get; set; }

        /// <summary>
        /// Gets or sets the sectors.
        /// </summary>
        public RuleFilter Sectors { get; set; }

        /// <summary>
        /// Gets the market cap filter.
        /// </summary>
        public DecimalRangeRuleFilter MarketCapFilter { get; }

        /// <summary>
        /// Gets the venue volume filter.
        /// </summary>
        public DecimalRangeRuleFilter VenueVolumeFilter { get; }

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
            if (obj == null) return false;

            var castObj = obj as HighVolumeIssuanceRuleFixedIncomeParameters;

            if (castObj == null) return false;

            return castObj.Windows == this.Windows
                   && castObj.FixedIncomeHighVolumePercentageWindow == this.FixedIncomeHighVolumePercentageDaily
                   && castObj.FixedIncomeHighVolumePercentageWindow == this.FixedIncomeHighVolumePercentageWindow;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Windows.GetHashCode() * this.FixedIncomeHighVolumePercentageDaily.GetHashCode()
                                              * this.FixedIncomeHighVolumePercentageWindow.GetHashCode();
        }

        /// <summary>
        /// The has internal filters check.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasInternalFilters()
        {
            return FilterableRuleExtensions.HasInternalFilters(this);
        }

        /// <summary>
        /// The valid settings check.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(this.Id)
                && (this.FixedIncomeHighVolumePercentageWindow == null
                       || (this.FixedIncomeHighVolumePercentageWindow.GetValueOrDefault() >= 0
                       && this.FixedIncomeHighVolumePercentageWindow.GetValueOrDefault() <= 1))
                   && (this.FixedIncomeHighVolumePercentageDaily == null
                       || (this.FixedIncomeHighVolumePercentageDaily.GetValueOrDefault() >= 0
                        && this.FixedIncomeHighVolumePercentageDaily.GetValueOrDefault() <= 1));
        }

        /// <summary>
        /// The has reference data filters.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasReferenceDataFilters()
        {
            return ReferenceDataFilterableExtensions.HasReferenceDataFilters(this);
        }

        /// <summary>
        /// The has market cap filters.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasMarketCapFilters()
        {
            return MarketCapFilterableExtensions.HasMarketCapFilters(this);
        }

        /// <summary>
        /// The has venue volume filters.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasVenueVolumeFilters()
        {
            return HighVolumeFilterableExtensions.HasVenueVolumeFilters(this);
        }
    }
}