namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Tuning;

    using Surveillance.Engine.Rules.RuleParameters.Extensions;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

    [Serializable]
    public class MarkingTheCloseEquitiesParameters : IMarkingTheCloseEquitiesParameters
    {
        public MarkingTheCloseEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageThresholdDailyVolume,
            decimal? percentageThresholdWindowVolume,
            decimal? percentThresholdOffTouch,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            this.Id = id ?? string.Empty;

            this.Windows = new TimeWindows(id, windowSize);
            this.PercentageThresholdDailyVolume = percentageThresholdDailyVolume;
            this.PercentageThresholdWindowVolume = percentageThresholdWindowVolume;
            this.PercentThresholdOffTouch = percentThresholdOffTouch;

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

        public MarkingTheCloseEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageThresholdDailyVolume,
            decimal? percentageThresholdWindowVolume,
            decimal? percentThresholdOffTouch,
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
            this.PercentageThresholdDailyVolume = percentageThresholdDailyVolume;
            this.PercentageThresholdWindowVolume = percentageThresholdWindowVolume;
            this.PercentThresholdOffTouch = percentThresholdOffTouch;

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

        [TuneableIdParameter]
        public string Id { get; set; }

        public RuleFilter Industries { get; set; }

        public DecimalRangeRuleFilter MarketCapFilter { get; }

        public RuleFilter Markets { get; set; }

        /// <summary>
        ///     A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? PercentageThresholdDailyVolume { get; set; }

        /// <summary>
        ///     A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? PercentageThresholdWindowVolume { get; set; }

        /// <summary>
        ///     A fractional percentage for how far from touch e.g. % away from bid for a buy; % away from ask for a sell
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? PercentThresholdOffTouch { get; set; }

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

            var castObj = obj as MarkingTheCloseEquitiesParameters;

            if (castObj == null) return false;

            return this.Windows == castObj.Windows
                   && this.PercentageThresholdDailyVolume == castObj.PercentageThresholdDailyVolume
                   && this.PercentageThresholdWindowVolume == castObj.PercentageThresholdWindowVolume
                   && this.PercentThresholdOffTouch == castObj.PercentThresholdOffTouch;
        }

        public override int GetHashCode()
        {
            return this.Windows.GetHashCode() * this.PercentageThresholdDailyVolume.GetHashCode()
                                              * this.PercentageThresholdWindowVolume.GetHashCode()
                                              * this.PercentThresholdOffTouch.GetHashCode();
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
                   && (this.PercentageThresholdDailyVolume == null
                       || this.PercentageThresholdDailyVolume.GetValueOrDefault() >= 0
                       && this.PercentageThresholdDailyVolume.GetValueOrDefault() <= 1)
                   && (this.PercentageThresholdWindowVolume == null
                       || this.PercentageThresholdWindowVolume.GetValueOrDefault() >= 0
                       && this.PercentageThresholdWindowVolume.GetValueOrDefault() <= 1)
                   && (this.PercentThresholdOffTouch == null || this.PercentThresholdOffTouch.GetValueOrDefault() >= 0
                       && this.PercentThresholdOffTouch.GetValueOrDefault() <= 1);
        }
    }
}