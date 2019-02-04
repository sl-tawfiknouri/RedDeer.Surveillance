namespace Surveillance.Specflow.Tests.StepDefinitions.HighVolume
{
    public class HighVolumeApiParameters
    {
        public string Id { get; set; }
        public int WindowHours { get; set; }
        public decimal? HighVolumePercentageDaily { get; set; }
        public decimal? HighVolumePercentageWindow { get; set; }
        public decimal? HighVolumePercentageMarketCap { get; set; }
    }
}
