namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.HighVolume
{
    /// <summary>
    /// The fixed income high volume issuance parameters.
    /// </summary>
    public class FixedIncomeHighVolumeApiParameters
    {
        /// <summary>
        /// Gets or sets the window hours.
        /// </summary>
        public int WindowHours { get; set; }

        /// <summary>
        /// Gets or sets the fixed income high volume percentage daily.
        /// </summary>
        public decimal? FixedIncomeHighVolumePercentageDaily { get; set; }

        /// <summary>
        /// Gets or sets the fixed income high volume percentage window.
        /// </summary>
        public decimal? FixedIncomeHighVolumePercentageWindow { get; set; }
    }
}