namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.HighProfit
{
    /// <summary>
    /// The fixed income high profit parameters.
    /// </summary>
    public class FixedIncomeHighProfitApiParameters
    {
        /// <summary>
        /// Gets or sets the future hours.
        /// </summary>
        public int FutureHours { get; set; }

        /// <summary>
        /// Gets or sets the high profit absolute.
        /// </summary>
        public decimal? HighProfitAbsolute { get; set; }

        /// <summary>
        /// Gets or sets the high profit currency.
        /// </summary>
        public string HighProfitCurrency { get; set; }

        /// <summary>
        /// Gets or sets the high profit percentage.
        /// </summary>
        public decimal? HighProfitPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether high profit use currency conversions.
        /// </summary>
        public bool HighProfitUseCurrencyConversions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether perform high profit daily analysis.
        /// </summary>
        public bool PerformHighProfitDailyAnalysis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether perform high profit window analysis.
        /// </summary>
        public bool PerformHighProfitWindowAnalysis { get; set; }

        /// <summary>
        /// Gets or sets the window hours.
        /// </summary>
        public int WindowHours { get; set; }
    }
}