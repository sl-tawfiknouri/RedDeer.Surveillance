namespace Surveillance.Specflow.Tests.StepDefinitions.HighVolume
{
    public class HighVolumeApiParameters
    {
        public decimal? HighVolumePercentageDaily { get; set; }

        public decimal? HighVolumePercentageMarketCap { get; set; }

        public decimal? HighVolumePercentageWindow { get; set; }

        public string Id { get; set; }

        public int WindowHours { get; set; }
    }
}