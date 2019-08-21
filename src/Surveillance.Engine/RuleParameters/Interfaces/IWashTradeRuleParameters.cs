namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IWashTradeRuleParameters : IRuleParameter
    {
        decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; }

        string AveragePositionMaximumAbsoluteValueChangeCurrency { get; }

        decimal? AveragePositionMaximumPositionValueChange { get; }

        // Parameter set one
        int? AveragePositionMinimumNumberOfTrades { get; }

        decimal? ClusteringPercentageValueDifferenceThreshold { get; }

        // Parameter set two
        int? ClusteringPositionMinimumNumberOfTrades { get; }

        bool PerformAveragePositionAnalysis { get; }

        bool PerformClusteringPositionAnalysis { get; }
    }
}