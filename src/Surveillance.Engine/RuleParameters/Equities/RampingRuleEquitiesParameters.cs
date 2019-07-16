using System;
using System.Collections.Generic;
using Domain.Surveillance.Rules.Tuning;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Extensions;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    [Serializable]
    public class RampingRuleEquitiesParameters : IRampingRuleEquitiesParameters
    {
        public RampingRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal autoCorrelationCoefficient,
            int? thresholdOrdersExecutedInWindow,
            decimal? thresholdVolumePercentageWindow,
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
            Id = id;
            Windows = new TimeWindows(id, windowSize);

            AutoCorrelationCoefficient = autoCorrelationCoefficient;
            ThresholdOrdersExecutedInWindow = thresholdOrdersExecutedInWindow;
            ThresholdVolumePercentageWindow = thresholdVolumePercentageWindow;

            MarketCapFilter = marketCapFilter ?? DecimalRangeRuleFilter.None();
            VenueVolumeFilter = venueVolumeFilter ?? DecimalRangeRuleFilter.None();

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategy ?? RuleFilter.None();

            Sectors = sectors ?? RuleFilter.None();
            Industries = industries ?? RuleFilter.None();
            Regions = regions ?? RuleFilter.None();
            Countries = countries ?? RuleFilter.None();

            Factors = factors;
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            PerformTuning = performTuning;
        }

        public RampingRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal autoCorrelationCoefficient,
            int? thresholdOrdersExecutedInWindow,
            decimal? thresholdVolumePercentageWindow,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            Id = id;
            Windows = new TimeWindows(id, windowSize);

            AutoCorrelationCoefficient = autoCorrelationCoefficient;
            ThresholdOrdersExecutedInWindow = thresholdOrdersExecutedInWindow;
            ThresholdVolumePercentageWindow = thresholdVolumePercentageWindow;

            MarketCapFilter = DecimalRangeRuleFilter.None();
            VenueVolumeFilter = DecimalRangeRuleFilter.None();

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Sectors = RuleFilter.None();
            Industries = RuleFilter.None();
            Regions = RuleFilter.None();
            Countries = RuleFilter.None();

            Factors = factors;
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            PerformTuning = performTuning;
        }

        [TuneableIdParameter]
        public string Id { get; set; }
        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }
        [TuneableIntegerParameter]
        public int? ThresholdOrdersExecutedInWindow { get; set; }
        [TuneableDecimalParameter]
        public decimal AutoCorrelationCoefficient { get; set; }
        [TuneableDecimalParameter]
        public decimal? ThresholdVolumePercentageWindow { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public DecimalRangeRuleFilter MarketCapFilter { get; set; }
        public DecimalRangeRuleFilter VenueVolumeFilter { get; set; }

        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }

        public RuleFilter Sectors { get; set; }
        public RuleFilter Industries { get; set; }
        public RuleFilter Regions { get; set; }
        public RuleFilter Countries { get; set; }

        public bool HasInternalFilters()
            => IFilterableRuleExtensions.HasInternalFilters(this);

        public bool HasMarketCapFilters()
            => IMarketCapFilterableExtensions.HasMarketCapFilters(this);

        public bool HasReferenceDataFilters()
            => IReferenceDataFilterableExtensions.HasReferenceDataFilters(this);

        public bool HasVenueVolumeFilters() 
            => IHighVolumeFilterableExtensions.HasVenueVolumeFilters(this);

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && (ThresholdOrdersExecutedInWindow == null
                    || ThresholdOrdersExecutedInWindow.GetValueOrDefault() >= 0)
                && AutoCorrelationCoefficient >= 0
                && (ThresholdVolumePercentageWindow == null
                    || (ThresholdVolumePercentageWindow >= 0
                        && ThresholdVolumePercentageWindow <= 1));
        }

        public override int GetHashCode()
        {
            return Windows.GetHashCode()
               * ThresholdOrdersExecutedInWindow.GetHashCode()
               * AutoCorrelationCoefficient.GetHashCode()
               * ThresholdVolumePercentageWindow.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as RampingRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return Windows == castObj.Windows
                   && ThresholdOrdersExecutedInWindow == castObj.ThresholdOrdersExecutedInWindow
                   && AutoCorrelationCoefficient == castObj.AutoCorrelationCoefficient
                   && ThresholdVolumePercentageWindow == castObj.ThresholdVolumePercentageWindow;
        }

        public bool PerformTuning { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }
    }
}
