namespace Surveillance.Specflow.Tests.StepDefinitions.Ramping
{
    public class RampingApiParameters
    {
        /// <summary>
        ///     AutoCorrelation Coefficient
        /// </summary>
        public decimal AutoCorrelationCoefficient { get; set; }

        /// <summary>
        ///     Materiality check on orders executed in the time window
        /// </summary>
        public int? ThresholdOrdersExecutedInWindow { get; set; }

        /// <summary>
        ///     Materiality check on the volume as a percentage of market cap in the time window
        /// </summary>
        public decimal? ThresholdVolumePercentageWindow { get; set; }

        public int WindowHours { get; set; }
    }
}