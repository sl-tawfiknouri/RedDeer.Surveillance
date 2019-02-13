using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters
{
    public class WashTradeRuleParameters : IWashTradeRuleParameters
    {
        public WashTradeRuleParameters(
            string id,
            TimeSpan windowSize,
            bool performAveragePositionAnalysis,
            bool performPairingPositionAnalysis,
            bool performClusteringPositionAnalysis,
            int? averagePositionMinimumNumberOfTrades,
            decimal? averagePositionMaximumPositionValueChange,
            decimal? averagePositionMaximumAbsoluteValueChangeAmount,
            string averagePositionMaximumAbsoluteValueChangeCurrency,
            int? pairingPositionMinimumNumberOfPairedTrades,
            decimal? pairingPositionPercentagePriceChangeThresholdPerPair,
            decimal? pairingPositionPercentageVolumeDifferenceThreshold,
            decimal? pairingPositionMaximumAbsoluteCurrencyAmount,
            string pairingPositionMaximumAbsoluteCurrency,
            int? clusteringPositionMinimumNumberOfTrades,
            decimal? clusteringPercentageValueDifferenceThreshold,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            WindowSize = windowSize;

            PerformAveragePositionAnalysis = performAveragePositionAnalysis;
            PerformPairingPositionAnalysis = performPairingPositionAnalysis;
            PerformClusteringPositionAnalysis = performClusteringPositionAnalysis;

            AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            AveragePositionMaximumAbsoluteValueChangeAmount = averagePositionMaximumAbsoluteValueChangeAmount;
            AveragePositionMaximumAbsoluteValueChangeCurrency = averagePositionMaximumAbsoluteValueChangeCurrency;

            PairingPositionMinimumNumberOfPairedTrades = pairingPositionMinimumNumberOfPairedTrades;
            PairingPositionPercentagePriceChangeThresholdPerPair = pairingPositionPercentagePriceChangeThresholdPerPair;
            PairingPositionPercentageVolumeDifferenceThreshold = pairingPositionPercentageVolumeDifferenceThreshold;
            PairingPositionMaximumAbsoluteCurrencyAmount = pairingPositionMaximumAbsoluteCurrencyAmount;
            PairingPositionMaximumAbsoluteCurrency = pairingPositionMaximumAbsoluteCurrency;

            ClusteringPositionMinimumNumberOfTrades = clusteringPositionMinimumNumberOfTrades;
            ClusteringPercentageValueDifferenceThreshold = clusteringPercentageValueDifferenceThreshold;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public WashTradeRuleParameters(
            string id,
            TimeSpan windowSize,
            bool performAveragePositionAnalysis,
            bool performPairingPositionAnalysis,
            bool performClusteringPositionAnalysis,
            int? averagePositionMinimumNumberOfTrades,
            decimal? averagePositionMaximumPositionValueChange,
            decimal? averagePositionMaximumAbsoluteValueChangeAmount,
            string averagePositionMaximumAbsoluteValueChangeCurrency,
            int? pairingPositionMinimumNumberOfPairedTrades,
            decimal? pairingPositionPercentagePriceChangeThresholdPerPair,
            decimal? pairingPositionPercentageVolumeDifferenceThreshold,
            decimal? pairingPositionMaximumAbsoluteCurrencyAmount,
            string pairingPositionMaximumAbsoluteCurrency,
            int? clusteringPositionMinimumNumberOfTrades,
            decimal? clusteringPercentageValueDifferenceThreshold,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            WindowSize = windowSize;

            PerformAveragePositionAnalysis = performAveragePositionAnalysis;
            PerformPairingPositionAnalysis = performPairingPositionAnalysis;
            PerformClusteringPositionAnalysis = performClusteringPositionAnalysis;

            AveragePositionMinimumNumberOfTrades = averagePositionMinimumNumberOfTrades;
            AveragePositionMaximumPositionValueChange = averagePositionMaximumPositionValueChange;
            AveragePositionMaximumAbsoluteValueChangeAmount = averagePositionMaximumAbsoluteValueChangeAmount;
            AveragePositionMaximumAbsoluteValueChangeCurrency = averagePositionMaximumAbsoluteValueChangeCurrency;

            PairingPositionMinimumNumberOfPairedTrades = pairingPositionMinimumNumberOfPairedTrades;
            PairingPositionPercentagePriceChangeThresholdPerPair = pairingPositionPercentagePriceChangeThresholdPerPair;
            PairingPositionPercentageVolumeDifferenceThreshold = pairingPositionPercentageVolumeDifferenceThreshold;
            PairingPositionMaximumAbsoluteCurrencyAmount = pairingPositionMaximumAbsoluteCurrencyAmount;
            PairingPositionMaximumAbsoluteCurrency = pairingPositionMaximumAbsoluteCurrency;

            ClusteringPositionMinimumNumberOfTrades = clusteringPositionMinimumNumberOfTrades;
            ClusteringPercentageValueDifferenceThreshold = clusteringPercentageValueDifferenceThreshold;

            Accounts = accounts;
            Traders = traders;
            Markets = markets;

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public string Id { get; }

        public TimeSpan WindowSize { get; set; }

        // Enabled analysis settings
        public bool PerformAveragePositionAnalysis { get; }
        public bool PerformPairingPositionAnalysis { get; }
        public bool PerformClusteringPositionAnalysis { get; }


        // Averaging parameters
        public int? AveragePositionMinimumNumberOfTrades { get; }
        public decimal? AveragePositionMaximumPositionValueChange { get; }
        public decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; }
        public string AveragePositionMaximumAbsoluteValueChangeCurrency { get; }

        // Pairing parameters
        public int? PairingPositionMinimumNumberOfPairedTrades { get; }
        public decimal? PairingPositionPercentagePriceChangeThresholdPerPair { get; }
        public decimal? PairingPositionPercentageVolumeDifferenceThreshold { get; }
        public decimal? PairingPositionMaximumAbsoluteCurrencyAmount { get; }
        public string PairingPositionMaximumAbsoluteCurrency { get; }
        
        // Clustering (k-means) parameters
        public int? ClusteringPositionMinimumNumberOfTrades { get; }
        public decimal? ClusteringPercentageValueDifferenceThreshold { get; }


        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool HasFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None;
        }
    }
}
