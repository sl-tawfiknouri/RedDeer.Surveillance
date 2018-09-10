using System;
using System.Collections.Generic;

namespace Surveillance.Scheduler
{
    /// <summary>
    /// DTO type for messages
    /// </summary>
    public class ScheduledExecution
    {
        /// <summary>
        /// Rule to run
        /// </summary>
        public List<Rules.Rules> Rules { get; set; } = new List<Rules.Rules>();

        /// <summary>
        /// Filter to apply to the data set time series
        /// </summary>
        public DateTimeOffset TimeSeriesInitiation { get; set; }

        /// <summary>
        /// Filter to apply to the data set time series
        /// </summary>
        public DateTimeOffset TimeSeriesTermination { get; set; }
    }
}
