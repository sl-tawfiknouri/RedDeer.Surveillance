using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface IWashTradeRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable
    {
        TimeSpan WindowSize { get; }

        bool PerformAveragePositionAnalysis { get; }
        bool PerformPairingPositionAnalysis { get; }
        bool PerformClusteringPositionAnalysis { get; }

        // Parameter set one
        int? AveragePositionMinimumNumberOfTrades { get; }
        decimal? AveragePositionMaximumPositionValueChange { get; }
        decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; }
        string AveragePositionMaximumAbsoluteValueChangeCurrency { get; }

        // Parameter set two
        int? PairingPositionMinimumNumberOfPairedTrades { get; }
        decimal? PairingPositionPercentagePriceChangeThresholdPerPair { get; }
        decimal? PairingPositionPercentageVolumeDifferenceThreshold { get; }
        decimal? PairingPositionMaximumAbsoluteMoney { get; }
        string PairingPositionMaximumAbsoluteCurrency { get; }

        // Parameter set three
        int? ClusteringPositionMinimumNumberOfTrades { get; }
        decimal? ClusteringPercentageValueDifferenceThreshold { get; }
    }
}
