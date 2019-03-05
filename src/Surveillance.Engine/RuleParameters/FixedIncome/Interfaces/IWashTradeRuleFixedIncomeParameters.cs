using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces
{
    public interface IWashTradeRuleFixedIncomeParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable
    {
        TimeSpan WindowSize { get; }

        bool PerformAveragePositionAnalysis { get; }
        bool PerformClusteringPositionAnalysis { get; }

        // Parameter set one
        int? AveragePositionMinimumNumberOfTrades { get; }
        decimal? AveragePositionMaximumPositionValueChange { get; }
        decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; }
        string AveragePositionMaximumAbsoluteValueChangeCurrency { get; }

        // Parameter set two
        int? ClusteringPositionMinimumNumberOfTrades { get; }
        decimal? ClusteringPercentageValueDifferenceThreshold { get; }
    }
}
