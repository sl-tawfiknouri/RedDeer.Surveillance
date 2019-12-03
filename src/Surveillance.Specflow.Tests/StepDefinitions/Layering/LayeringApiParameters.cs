namespace Surveillance.Specflow.Tests.StepDefinitions.Spoofing
{
    public class LayeringApiParameters
    {
        public bool? CheckForCorrespondingPriceMovement { get; set; }
        public decimal? PercentageOfMarketDailyVolume { get; set; }
        public decimal? PercentageOfMarketWindowVolume { get; set; }
        public int WindowHours { get; set; }
    }
}