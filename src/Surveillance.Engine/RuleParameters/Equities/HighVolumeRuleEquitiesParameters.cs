namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Tuning;

    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Extensions;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    [Serializable]
    public class HighVolumeRuleEquitiesParameters : IHighVolumeRuleEquitiesParameters
    {
        public HighVolumeRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow,
            decimal? highVolumePercentageMarketCap,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            this.Id = id ?? string.Empty;

            this.Windows = new TimeWindows(id, windowSize);
            this.HighVolumePercentageDaily = highVolumePercentageDaily;
            this.HighVolumePercentageWindow = highVolumePercentageWindow;
            this.HighVolumePercentageMarketCap = highVolumePercentageMarketCap;

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

        public HighVolumeRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow,
            decimal? highVolumePercentageMarketCap,
            DecimalRangeRuleFilter marketCapFilter,
            DecimalRangeRuleFilter venueVolumeFilter,
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
            this.HighVolumePercentageDaily = highVolumePercentageDaily;
            this.HighVolumePercentageWindow = highVolumePercentageWindow;
            this.HighVolumePercentageMarketCap = highVolumePercentageMarketCap;

            this.MarketCapFilter = marketCapFilter ?? DecimalRangeRuleFilter.None();
            this.VenueVolumeFilter = venueVolumeFilter ?? DecimalRangeRuleFilter.None();

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

        public RuleFilter Accounts { get; set; }

        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public RuleFilter Countries { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        public RuleFilter Funds { get; set; }

        [TuneableDecimalParameter]
        public decimal? HighVolumePercentageDaily { get; set; }

        [TuneableDecimalParameter]
        public decimal? HighVolumePercentageMarketCap { get; set; }

        [TuneableDecimalParameter]
        public decimal? HighVolumePercentageWindow { get; set; }

        [TuneableIdParameter]
        public string Id { get; set; }

        public RuleFilter Industries { get; set; }

        public DecimalRangeRuleFilter MarketCapFilter { get; }

        public RuleFilter Markets { get; set; }

        public bool PerformTuning { get; set; }

        public RuleFilter Regions { get; set; }

        public RuleFilter Sectors { get; set; }

        public RuleFilter Strategies { get; set; }

        public RuleFilter Traders { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParameters { get; set; }

        public DecimalRangeRuleFilter VenueVolumeFilter { get; set; }

        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var castObj = obj as HighVolumeRuleEquitiesParameters;

            if (castObj == null) return false;

            return this.Windows == castObj.Windows
                   && this.HighVolumePercentageDaily == castObj.HighVolumePercentageDaily
                   && this.HighVolumePercentageMarketCap == castObj.HighVolumePercentageMarketCap
                   && this.HighVolumePercentageWindow == castObj.HighVolumePercentageWindow;
        }

        public override int GetHashCode()
        {
            return this.Windows.GetHashCode() * this.HighVolumePercentageDaily.GetHashCode()
                                              * this.HighVolumePercentageWindow.GetHashCode()
                                              * this.HighVolumePercentageMarketCap.GetHashCode();
        }

        public bool HasInternalFilters()
        {
            return FilterableRuleExtensions.HasInternalFilters(this);
        }

        public bool HasMarketCapFilters()
        {
            return MarketCapFilterableExtensions.HasMarketCapFilters(this);
        }

        public bool HasReferenceDataFilters()
        {
            return ReferenceDataFilterableExtensions.HasReferenceDataFilters(this);
        }

        public bool HasVenueVolumeFilters()
        {
            return HighVolumeFilterableExtensions.HasVenueVolumeFilters(this);
        }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(this.Id)
                   && (this.HighVolumePercentageDaily == null || this.HighVolumePercentageDaily.GetValueOrDefault() >= 0
                       && this.HighVolumePercentageDaily.GetValueOrDefault() <= 1)
                   && (this.HighVolumePercentageWindow == null
                       || this.HighVolumePercentageWindow.GetValueOrDefault() >= 0
                       && this.HighVolumePercentageWindow.GetValueOrDefault() <= 1)
                   && (this.HighVolumePercentageMarketCap == null
                       || this.HighVolumePercentageMarketCap.GetValueOrDefault() >= 0
                       && this.HighVolumePercentageMarketCap.GetValueOrDefault() <= 1);
        }
    }
}