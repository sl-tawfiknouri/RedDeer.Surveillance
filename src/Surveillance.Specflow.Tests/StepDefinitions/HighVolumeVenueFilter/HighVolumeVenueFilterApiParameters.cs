namespace Surveillance.Specflow.Tests.StepDefinitions.HighVolumeVenueFilter
{
    public class HighVolumeVenueFilterApiParameters
    {
        public string Id { get; set; }
        public int WindowHours { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
