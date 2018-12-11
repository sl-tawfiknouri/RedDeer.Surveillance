using System;
using System.Collections.Generic;

namespace DomainV2.Scheduling
{
    /// <summary>
    /// DTO type for messages
    /// </summary>
    public class ScheduledExecution
    {
        /// <summary>
        /// Rule to run
        /// </summary>
        public List<RuleIdentifier> Rules { get; set; } = new List<RuleIdentifier>();

        /// <summary>
        /// Filter to apply to the data set time series
        /// </summary>
        public DateTimeOffset TimeSeriesInitiation { get; set; }

        /// <summary>
        /// Filter to apply to the data set time series
        /// </summary>
        public DateTimeOffset TimeSeriesTermination { get; set; }

        /// <summary>
        /// Typically the op id of the distributed rule
        /// </summary>
        public string CorrelationId { get; set; }
    }
}
