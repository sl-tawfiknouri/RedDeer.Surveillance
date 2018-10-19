using System;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class HighVolumeRuleParameters : IHighVolumeRuleParameters
    {
        public HighVolumeRuleParameters(
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow)
        {
            WindowSize = windowSize;
            HighVolumePercentageDaily = highVolumePercentageDaily;
            HighVolumePercentageWindow = highVolumePercentageWindow;
        }

        public TimeSpan WindowSize { get; }
        public decimal? HighVolumePercentageDaily { get; }
        public decimal? HighVolumePercentageWindow { get; }
    }
}
