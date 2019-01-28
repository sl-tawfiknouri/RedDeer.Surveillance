namespace Surveillance.Specflow.Tests.StepDefinitions.WashTrades
{
    public class WashTradeApiParameters
    {
        public int WindowHours { get; set; }

        // Average Netting
        public bool? UseAverageNetting { get; set; }
        public int? MinimumNumberOfTrades { get; set; }
        public decimal? MaximumPositionChangeValue { get; set; }
        public int? MaximumAbsoluteValueChange { get; set; }
        public string MaximumAbsoluteValueChangeCurrency { get; set; }

        // Clustering
        public bool? UseClustering { get; set; }
        public int? ClusteringPositionMinimumNumberOfTrades { get; set; }
        public decimal? ClusteringPercentageValueDifferenceThreshold { get; set; }


        // Pairing
        public bool? UsePairing { get; set; }
        public int? PairingPositionMinimumNumberOfPairedTrades { get; set; }
        public decimal? PairingPositionPercentagePriceChangeThresholdPerPair { get; set; }
        public decimal? PairingPositionPercentageVolumeDifferenceThreshold { get; set; }
        public decimal? PairingPositionMaximumAbsoluteCurrencyAmount { get; set; }
        public string PairingPositionMaximumAbsoluteCurrency { get; set; }
    }
}
