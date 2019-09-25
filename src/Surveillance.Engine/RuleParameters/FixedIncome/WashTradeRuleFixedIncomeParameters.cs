namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Tuning;

    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

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

            this.Accounts = accounts ?? RuleFilter.None();
            this.Traders = traders ?? RuleFilter.None();
            this.Markets = markets ?? RuleFilter.None();
            this.Funds = funds ?? RuleFilter.None();
            this.Strategies = strategies ?? RuleFilter.None();
            this.Factors = factors ?? new List<ClientOrganisationalFactors>();
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

        [TuneableIntegerParameter]
        public int? AveragePositionMinimumNumberOfTrades { get; set; }

        [TuneableDecimalParameter]
        public decimal? ClusteringPercentageValueDifferenceThreshold { get; set; }

        [TuneableIntegerParameter]
        public int? ClusteringPositionMinimumNumberOfTrades { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        public RuleFilter Funds { get; set; }

        [TuneableIdParameter]
        public string Id { get; set; }

        public RuleFilter Markets { get; set; }

        public string PairingPositionMaximumAbsoluteCurrency => null;

        public decimal? PairingPositionMaximumAbsoluteMoney => null;

        public int? PairingPositionMinimumNumberOfPairedTrades => null;

        public decimal? PairingPositionPercentagePriceChangeThresholdPerPair => null;

        public decimal? PairingPositionPercentageVolumeDifferenceThreshold => null;

        public bool PerformAveragePositionAnalysis { get; set; }

        public bool PerformClusteringPositionAnalysis { get; set; }

        // Removing from wash trade parameter interface soon
        public bool PerformPairingPositionAnalysis => false;

        public bool PerformTuning { get; set; }

        public RuleFilter Strategies { get; set; }

        public RuleFilter Traders { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParameters { get; set; }

        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var castObj = obj as WashTradeRuleFixedIncomeParameters;

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
            return this.Accounts?.Type != RuleFilterType.None || this.Traders?.Type != RuleFilterType.None
                                                              || this.Markets?.Type != RuleFilterType.None;
        }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(this.Id)
                   && (this.AveragePositionMinimumNumberOfTrades == null
                       || this.AveragePositionMinimumNumberOfTrades >= 0)
                   && (this.AveragePositionMaximumPositionValueChange == null
                       || this.AveragePositionMaximumPositionValueChange >= 0)
                   && (this.AveragePositionMaximumAbsoluteValueChangeAmount == null
                       || this.AveragePositionMaximumAbsoluteValueChangeAmount >= 0)
                   && (this.ClusteringPositionMinimumNumberOfTrades == null
                       || this.ClusteringPositionMinimumNumberOfTrades >= 0)
                   && (this.ClusteringPercentageValueDifferenceThreshold == null
                       || this.ClusteringPercentageValueDifferenceThreshold >= 0);
        }
    }
}