namespace Surveillance.Specflow.Tests.StepDefinitions.WashTrades
{
    public class WashTradeApiParameters
    {
        public decimal? ClusteringPercentageValueDifferenceThreshold { get; set; }

        public int? ClusteringPositionMinimumNumberOfTrades { get; set; }

        public int? MaximumAbsoluteValueChange { get; set; }

        public string MaximumAbsoluteValueChangeCurrency { get; set; }

        public decimal? MaximumPositionChangeValue { get; set; }

        public int? MinimumNumberOfTrades { get; set; }

        // Average Netting
        public bool? UseAverageNetting { get; set; }

        // Clustering
        public bool? UseClustering { get; set; }

        public int WindowHours { get; set; }
    }
}