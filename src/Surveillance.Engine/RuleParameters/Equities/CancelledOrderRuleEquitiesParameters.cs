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
    public class CancelledOrderRuleEquitiesParameters : ICancelledOrderRuleEquitiesParameters
    {
        public CancelledOrderRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? cancelledOrderPositionPercentageThreshold,
            decimal? cancelledOrderCountPercentageThreshold,
            int minimumNumberOfTradesToApplyRuleTo,
            int? maximumNumberOfTradesToApplyRuleTo,
            DecimalRangeRuleFilter marketCapFilter,
            DecimalRangeRuleFilter venueVolumeFilter,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategy,
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
            this.CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            this.CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            this.MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            this.MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;

            this.MarketCapFilter = marketCapFilter ?? DecimalRangeRuleFilter.None();
            this.VenueVolumeFilter = venueVolumeFilter ?? DecimalRangeRuleFilter.None();

            this.Accounts = accounts ?? RuleFilter.None();
            this.Traders = traders ?? RuleFilter.None();
            this.Markets = markets ?? RuleFilter.None();
            this.Funds = funds ?? RuleFilter.None();
            this.Strategies = strategy ?? RuleFilter.None();

            this.Sectors = sectors ?? RuleFilter.None();
            this.Industries = industries ?? RuleFilter.None();
            this.Regions = regions ?? RuleFilter.None();
            this.Countries = countries ?? RuleFilter.None();

            this.Factors = factors ?? new ClientOrganisationalFactors[0];
            this.AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            this.PerformTuning = performTuning;
        }

        public CancelledOrderRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? cancelledOrderPositionPercentageThreshold,
            decimal? cancelledOrderCountPercentageThreshold,
            int minimumNumberOfTradesToApplyRuleTo,
            int? maximumNumberOfTradesToApplyRuleTo,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            this.Id = id ?? string.Empty;
            this.Windows = new TimeWindows(id, windowSize);
            this.CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            this.CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            this.MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            this.MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;

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

        public RuleFilter Accounts { get; set; }

        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        [TuneableDecimalParameter]
        public decimal? CancelledOrderCountPercentageThreshold { get; set; }

        [TuneableDecimalParameter]
        public decimal? CancelledOrderPercentagePositionThreshold { get; set; }

        public RuleFilter Countries { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        public RuleFilter Funds { get; set; }

        [TuneableIdParameter]
        public string Id { get; set; }

        public RuleFilter Industries { get; set; }

        public DecimalRangeRuleFilter MarketCapFilter { get; }

        public RuleFilter Markets { get; set; }

        [TuneableIntegerParameter]
        public int? MaximumNumberOfTradesToApplyRuleTo { get; set; }

        [TuneableIntegerParameter]
        public int MinimumNumberOfTradesToApplyRuleTo { get; set; }

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

            var castObj = obj as CancelledOrderRuleEquitiesParameters;

            if (castObj == null) return false;

            return this.Windows == castObj.Windows
                   && this.CancelledOrderPercentagePositionThreshold
                   == castObj.CancelledOrderPercentagePositionThreshold
                   && this.CancelledOrderCountPercentageThreshold == castObj.CancelledOrderCountPercentageThreshold
                   && this.MinimumNumberOfTradesToApplyRuleTo == castObj.MinimumNumberOfTradesToApplyRuleTo
                   && this.MaximumNumberOfTradesToApplyRuleTo == castObj.MaximumNumberOfTradesToApplyRuleTo;
        }

        public override int GetHashCode()
        {
            return this.Windows.GetHashCode() * this.CancelledOrderPercentagePositionThreshold.GetHashCode()
                                              * this.CancelledOrderCountPercentageThreshold.GetHashCode()
                                              * this.MinimumNumberOfTradesToApplyRuleTo.GetHashCode()
                                              * this.MaximumNumberOfTradesToApplyRuleTo.GetHashCode();
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
                   && (this.CancelledOrderPercentagePositionThreshold == null
                       || this.CancelledOrderPercentagePositionThreshold.Value <= 1
                       && this.CancelledOrderPercentagePositionThreshold.Value >= 0)
                   && (this.CancelledOrderCountPercentageThreshold == null
                       || this.CancelledOrderCountPercentageThreshold.Value >= 0
                       && this.CancelledOrderCountPercentageThreshold <= 1)
                   && this.MinimumNumberOfTradesToApplyRuleTo >= 2
                   && (this.MaximumNumberOfTradesToApplyRuleTo == null
                       || this.MaximumNumberOfTradesToApplyRuleTo.GetValueOrDefault()
                       >= this.MinimumNumberOfTradesToApplyRuleTo);
        }
    }
}