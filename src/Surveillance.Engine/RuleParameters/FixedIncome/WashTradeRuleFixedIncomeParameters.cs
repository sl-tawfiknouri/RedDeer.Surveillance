using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome
{
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
            Factors = factors ?? new List<ClientOrganisationalFactors>();
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public string Id { get; }
        public TimeWindows Windows { get; }

        public bool PerformAveragePositionAnalysis { get; }
        public bool PerformClusteringPositionAnalysis { get; }

        public int? AveragePositionMinimumNumberOfTrades { get; }
        public decimal? AveragePositionMaximumPositionValueChange { get; }
        public decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; }
        public string AveragePositionMaximumAbsoluteValueChangeCurrency { get; }

        public int? ClusteringPositionMinimumNumberOfTrades { get; }
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




        // Removing from wash trade parameter interface soon
        public bool PerformPairingPositionAnalysis => false;

        public int? PairingPositionMinimumNumberOfPairedTrades => null;

        public decimal? PairingPositionPercentagePriceChangeThresholdPerPair => null;

        public decimal? PairingPositionPercentageVolumeDifferenceThreshold => null;

        public decimal? PairingPositionMaximumAbsoluteMoney => null;

        public string PairingPositionMaximumAbsoluteCurrency => null;
    }
}
