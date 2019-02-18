namespace Surveillance.Specflow.Tests.StepDefinitions.MarkingTheClose
{
    public class MarkingTheCloseApiParameters
    {
        public int WindowHours { get; set; }

        /// <summary>
        /// A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        public decimal? PercentageThresholdDailyVolume { get; set; }

        /// <summary>
        /// A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        public decimal? PercentageThresholdWindowVolume { get; set; }
    }
}
