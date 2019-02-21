using System;
using System.Collections.Generic;

namespace Domain.Scheduling
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

        public bool IsBackTest { get; set; }

        /// <summary>
        /// Once we have as much market data as we can get, force a re-run
        /// </summary>
        public bool IsForceRerun { get; set; }

        /// <summary>
        /// Set in memory; largest timespan for relevant rules
        /// </summary>
        public TimeSpan LeadingTimespan { get; set; }
    }
}
