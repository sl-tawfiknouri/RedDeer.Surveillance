using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
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
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(windowSize);

            PerformAveragePositionAnalysis = performAveragePositionAnalysis;
            PerformClusteringPositionAnalysis = performClusteringPositionAnalysis;

            AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            AveragePositionMaximumAbsoluteValueChangeAmount = averagePositionMaximumAbsoluteValueChangeAmount;
            AveragePositionMaximumAbsoluteValueChangeCurrency = averagePositionMaximumAbsoluteValueChangeCurrency;

            ClusteringPositionMinimumNumberOfTrades = clusteringPositionMinimumNumberOfTrades;
            ClusteringPercentageValueDifferenceThreshold = clusteringPercentageValueDifferenceThreshold;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
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
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(windowSize);

            PerformAveragePositionAnalysis = performAveragePositionAnalysis;
            PerformClusteringPositionAnalysis = performClusteringPositionAnalysis;

            AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            AveragePositionMaximumAbsoluteValueChangeAmount = averagePositionMaximumAbsoluteValueChangeAmount;
            AveragePositionMaximumAbsoluteValueChangeCurrency = averagePositionMaximumAbsoluteValueChangeCurrency;

            ClusteringPositionMinimumNumberOfTrades = clusteringPositionMinimumNumberOfTrades;
            ClusteringPercentageValueDifferenceThreshold = clusteringPercentageValueDifferenceThreshold;

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
        public TimeWindows Windows { get; }

        // Enabled analysis settings
        public bool PerformAveragePositionAnalysis { get; set; }
        public bool PerformClusteringPositionAnalysis { get; set; }


        // Averaging parameters
        [TuneableIntegerParameter]
        public int? AveragePositionMinimumNumberOfTrades { get; set; }
        [TuneableDecimalParameter]
        public decimal? AveragePositionMaximumPositionValueChange { get; set; }
        [TuneableDecimalParameter]
        public decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; set; }
        public string AveragePositionMaximumAbsoluteValueChangeCurrency { get; set; }

        // Clustering (k-means) parameters
        [TuneableIntegerParameter]
        public int? ClusteringPositionMinimumNumberOfTrades { get; set; }
        [TuneableDecimalParameter]
        public decimal? ClusteringPercentageValueDifferenceThreshold { get; set; }


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
               && (AveragePositionMinimumNumberOfTrades == null
                   || (AveragePositionMinimumNumberOfTrades.GetValueOrDefault() >= 0))
                && (AveragePositionMaximumPositionValueChange == null
                    || AveragePositionMaximumPositionValueChange >= 0
                && (AveragePositionMaximumAbsoluteValueChangeAmount == null
                    || AveragePositionMaximumAbsoluteValueChangeAmount >= 0))
                && (ClusteringPositionMinimumNumberOfTrades == null
                    || ClusteringPositionMinimumNumberOfTrades >= 0)
                && (ClusteringPercentageValueDifferenceThreshold == null
                    || ClusteringPercentageValueDifferenceThreshold >= 0);
        }

        public override int GetHashCode()
        {
            return Windows.GetHashCode()
               * AveragePositionMinimumNumberOfTrades.GetHashCode()
               * AveragePositionMaximumPositionValueChange.GetHashCode()
               * AveragePositionMaximumAbsoluteValueChangeAmount.GetHashCode()
               * ClusteringPositionMinimumNumberOfTrades.GetHashCode()
               * ClusteringPercentageValueDifferenceThreshold.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as WashTradeRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return Windows == castObj.Windows
                   && AveragePositionMinimumNumberOfTrades == castObj.AveragePositionMinimumNumberOfTrades
                   && AveragePositionMaximumPositionValueChange == castObj.AveragePositionMaximumPositionValueChange
                   && AveragePositionMaximumAbsoluteValueChangeAmount == castObj.AveragePositionMaximumAbsoluteValueChangeAmount
                   && ClusteringPositionMinimumNumberOfTrades == castObj.ClusteringPositionMinimumNumberOfTrades
                   && ClusteringPercentageValueDifferenceThreshold == castObj.ClusteringPercentageValueDifferenceThreshold;
        }
    }
}
