using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
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
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategy,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id;
            Windows = new TimeWindows(windowSize);

            AutoCorrelationCoefficient = autoCorrelationCoefficient;
            ThresholdOrdersExecutedInWindow = thresholdOrdersExecutedInWindow;
            ThresholdVolumePercentageWindow = thresholdVolumePercentageWindow;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategy ?? RuleFilter.None();

            Factors = factors;
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public RampingRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal autoCorrelationCoefficient,
            int? thresholdOrdersExecutedInWindow,
            decimal? thresholdVolumePercentageWindow,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id;
            Windows = new TimeWindows(windowSize);

            AutoCorrelationCoefficient = autoCorrelationCoefficient;
            ThresholdOrdersExecutedInWindow = thresholdOrdersExecutedInWindow;
            ThresholdVolumePercentageWindow = thresholdVolumePercentageWindow;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Factors = factors;
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
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

        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }

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
    }
}
