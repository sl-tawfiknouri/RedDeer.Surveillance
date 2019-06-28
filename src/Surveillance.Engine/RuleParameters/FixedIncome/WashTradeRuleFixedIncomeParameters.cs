using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome
{
    [Serializable]
    public class WashTradeRuleFixedIncomeParameters : IWashTradeRuleFixedIncomeParameters
    {
        public WashTradeRuleFixedIncomeParameters(
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
            WindowSize = windowSize;

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
            Factors = factors ?? new List<ClientOrganisationalFactors>();
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        [TuneableIdParameter]
        public string Id { get; }
        [TuneableTimespanParameter]
        public TimeSpan WindowSize { get; }
        
        public bool PerformAveragePositionAnalysis { get; }
        public bool PerformClusteringPositionAnalysis { get; }
        
        [TuneableIntegerParameter]
        public int? AveragePositionMinimumNumberOfTrades { get; }
        [TuneableDecimalParameter]
        public decimal? AveragePositionMaximumPositionValueChange { get; }
        [TuneableDecimalParameter]
        public decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; }
        public string AveragePositionMaximumAbsoluteValueChangeCurrency { get; }

        [TuneableIntegerParameter]
        public int? ClusteringPositionMinimumNumberOfTrades { get; }
        [TuneableDecimalParameter]
        public decimal? ClusteringPercentageValueDifferenceThreshold { get; }

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
                || Markets?.Type != RuleFilterType.None;
        }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(Id)
               && (AveragePositionMinimumNumberOfTrades == null
                   || AveragePositionMinimumNumberOfTrades >= 0)
                && (AveragePositionMaximumPositionValueChange == null
                || AveragePositionMaximumPositionValueChange >= 0)
                && (AveragePositionMaximumAbsoluteValueChangeAmount == null
                    || AveragePositionMaximumAbsoluteValueChangeAmount >= 0)
                && (ClusteringPositionMinimumNumberOfTrades == null
                    || ClusteringPositionMinimumNumberOfTrades >= 0)
                && (ClusteringPercentageValueDifferenceThreshold == null
                    || ClusteringPercentageValueDifferenceThreshold >= 0);
        }

        // Removing from wash trade parameter interface soon
        public bool PerformPairingPositionAnalysis => false;

        public int? PairingPositionMinimumNumberOfPairedTrades => null;

        public decimal? PairingPositionPercentagePriceChangeThresholdPerPair => null;

        public decimal? PairingPositionPercentageVolumeDifferenceThreshold => null;

        public decimal? PairingPositionMaximumAbsoluteMoney => null;

        public string PairingPositionMaximumAbsoluteCurrency => null;
    }
}
