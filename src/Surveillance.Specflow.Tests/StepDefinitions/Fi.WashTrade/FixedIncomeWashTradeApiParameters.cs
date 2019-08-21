namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.WashTrade
{
    public class FixedIncomeWashTradeApiParameters
    {
        public decimal? AveragePositionMaximumAbsoluteValueChangeAmount { get; set; }

        public string AveragePositionMaximumAbsoluteValueChangeCurrency { get; set; }

        public decimal? AveragePositionMaximumPositionValueChange { get; set; }

        public int? AveragePositionMinimumNumberOfTrades { get; set; }

        public decimal? ClusteringPercentageValueDifferenceThreshold { get; set; }

        public int? ClusteringPositionMinimumNumberOfTrades { get; set; }

        public bool PerformAveragePositionAnalysis { get; set; }

        public bool PerformClusteringPositionAnalysis { get; set; }

        public int WindowHours { get; set; }
    }
}