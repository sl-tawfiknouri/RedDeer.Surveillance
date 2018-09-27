using System;
using Surveillance.Rules.Marking_The_Close.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class MarkingTheCloseParameters : IMarkingTheCloseParameters
    {
        public MarkingTheCloseParameters()
        {
            Window = TimeSpan.FromMinutes(30);
            PercentageThresholdDailyVolume = 0.01m;
            PercentageThresholdWindowVolume = 0.05m;
            PercentThresholdOffTouch = 0.02m;
        }

        public TimeSpan Window { get; }

        /// <summary>
        /// A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        public decimal? PercentageThresholdDailyVolume { get; }

        /// <summary>
        /// A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        public decimal? PercentageThresholdWindowVolume { get; }

        /// <summary>
        /// A fractional percentage for how far from touch e.g. % away from bid for a buy; % away from ask for a sell
        /// </summary>
        public decimal? PercentThresholdOffTouch { get; }

    }
}
