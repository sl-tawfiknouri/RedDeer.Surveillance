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
    public class WashTradeRuleEquitiesParameters : IWashTradeRuleEquitiesParameters
    {
        public WashTradeRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            bool performAveragePositionAnalysis,
            bool performClusteringPositionAnalysis,
            int? averagePositionMinimumNumberOfTrades,
            decimal? averagePositionMaximumPositionValueChange,
            decimal? averagePositionMaximumAbsoluteValueChangeAmount,
            string averagePositionMaximumAbsoluteValueChangeCurrency,
            int? clusteringPositionMinimumNumberOfTrades,
            decimal? clusteringPercentageValueDifferenceThreshold,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            this.Id = id ?? string.Empty;

            this.Windows = new TimeWindows(id, windowSize);

            this.PerformAveragePositionAnalysis = performAveragePositionAnalysis;
            this.PerformClusteringPositionAnalysis = performClusteringPositionAnalysis;

            this.AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            this.AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            this.AveragePositionMaximumAbsoluteValueChangeAmount = averagePositionMaximumAbsoluteValueChangeAmount;
            this.AveragePositionMaximumAbsoluteValueChangeCurrency = averagePositionMaximumAbsoluteValueChangeCurrency;

            this.ClusteringPositionMinimumNumberOfTrades = clusteringPositionMinimumNumberOfTrades;
            this.ClusteringPercentageValueDifferenceThreshold = clusteringPercentageValueDifferenceThreshold;

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

        public WashTradeRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            bool performAveragePositionAnalysis,
            bool performClusteringPositionAnalysis,
            int? averagePositionMinimumNumberOfTrades,
            decimal? averagePositionMaximumPositionValueChange,
            decimal? averagePositionMaximumAbsoluteValueChangeAmount,
            string averagePositionMaximumAbsoluteValueChangeCurrency,
            int? clusteringPositionMinimumNumberOfTrades,
            decimal? clusteringPercentageValueDifferenceThreshold,
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

            this.PerformAveragePositionAnalysis = performAveragePositionAnalysis;
            this.PerformClusteringPositionAnalysis = performClusteringPositionAnalysis;

            this.AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            this.AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            this.AveragePositionMaximumAbsoluteValueChangeAmount = averagePositionMaximumAbsoluteValueChangeAmount;
            this.AveragePositionMaximumAbsoluteValueChangeCurrency = averagePositionMaximumAbsoluteValueChangeCurrency;

            this.ClusteringPositionMinimumNumberOfTrades = clusteringPositionMinimumNumberOfTrades;
            this.ClusteringPercentageValueDifferenceThreshold = clusteringPercentageValueDifferenceThreshold;

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

        [TuneableDecimalParameter]
        public decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; set; }

        public string AveragePositionMaximumAbsoluteValueChangeCurrency { get; set; }

        [TuneableDecimalParameter]
        public decimal? AveragePositionMaximumPositionValueChange { get; set; }

        // Averaging parameters
        [TuneableIntegerParameter]
        public int? AveragePositionMinimumNumberOfTrades { get; set; }

        [TuneableDecimalParameter]
        public decimal? ClusteringPercentageValueDifferenceThreshold { get; set; }

        // Clustering (k-means) parameters
        [TuneableIntegerParameter]
        public int? ClusteringPositionMinimumNumberOfTrades { get; set; }

        public RuleFilter Countries { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        public RuleFilter Funds { get; set; }

        [TuneableIdParameter]
        public string Id { get; set; }

        public RuleFilter Industries { get; set; }

        public DecimalRangeRuleFilter MarketCapFilter { get; set; }

        public RuleFilter Markets { get; set; }

        // Enabled analysis settings
        public bool PerformAveragePositionAnalysis { get; set; }

        public bool PerformClusteringPositionAnalysis { get; set; }

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

            var castObj = obj as WashTradeRuleEquitiesParameters;

            if (castObj == null) return false;

            return this.Windows == castObj.Windows
                   && this.AveragePositionMinimumNumberOfTrades == castObj.AveragePositionMinimumNumberOfTrades
                   && this.AveragePositionMaximumPositionValueChange
                   == castObj.AveragePositionMaximumPositionValueChange
                   && this.AveragePositionMaximumAbsoluteValueChangeAmount
                   == castObj.AveragePositionMaximumAbsoluteValueChangeAmount
                   && this.ClusteringPositionMinimumNumberOfTrades == castObj.ClusteringPositionMinimumNumberOfTrades
                   && this.ClusteringPercentageValueDifferenceThreshold
                   == castObj.ClusteringPercentageValueDifferenceThreshold;
        }

        public override int GetHashCode()
        {
            return this.Windows.GetHashCode() * this.AveragePositionMinimumNumberOfTrades.GetHashCode()
                                              * this.AveragePositionMaximumPositionValueChange.GetHashCode()
                                              * this.AveragePositionMaximumAbsoluteValueChangeAmount.GetHashCode()
                                              * this.ClusteringPositionMinimumNumberOfTrades.GetHashCode()
                                              * this.ClusteringPercentageValueDifferenceThreshold.GetHashCode();
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
                   && (this.AveragePositionMinimumNumberOfTrades == null
                       || this.AveragePositionMinimumNumberOfTrades.GetValueOrDefault() >= 0)
                   && (this.AveragePositionMaximumPositionValueChange == null
                       || this.AveragePositionMaximumPositionValueChange >= 0
                       && (this.AveragePositionMaximumAbsoluteValueChangeAmount == null
                           || this.AveragePositionMaximumAbsoluteValueChangeAmount >= 0))
                   && (this.ClusteringPositionMinimumNumberOfTrades == null
                       || this.ClusteringPositionMinimumNumberOfTrades >= 0)
                   && (this.ClusteringPercentageValueDifferenceThreshold == null
                       || this.ClusteringPercentageValueDifferenceThreshold >= 0);
        }
    }
}