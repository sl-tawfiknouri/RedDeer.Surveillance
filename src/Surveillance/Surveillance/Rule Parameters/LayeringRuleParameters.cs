using System;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class LayeringRuleParameters : ILayeringRuleParameters
    {
        public LayeringRuleParameters(
            TimeSpan windowSize,
            decimal? percentageOfMarketDailyVolume)
        {
            WindowSize = windowSize;
            PercentageOfMarketDailyVolume = percentageOfMarketDailyVolume;
        }

        public TimeSpan WindowSize { get; }
        public decimal? PercentageOfMarketDailyVolume { get; }
    }
}
