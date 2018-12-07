﻿using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface IWashTradeRuleParameters : IFilterableRule
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
        decimal? PairingPositionMaximumAbsoluteCurrencyAmount { get; }
        string PairingPositionMaximumAbsoluteCurrency { get; }

        // Parameter set three
        int? ClusteringPositionMinimumNumberOfTrades { get; }
        decimal? ClusteringPercentageValueDifferenceThreshold { get; }

        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        bool AggregateNonFactorableIntoOwnCategory { get; set; }
    }
}