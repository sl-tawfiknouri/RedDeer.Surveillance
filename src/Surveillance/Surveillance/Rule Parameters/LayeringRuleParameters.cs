using System;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class LayeringRuleParameters : ILayeringRuleParameters
    {
        public LayeringRuleParameters(
            TimeSpan windowSize,
            decimal? percentageOfMarketVolume)
        {
            WindowSize = windowSize;
            PercentageOfMarketVolume = percentageOfMarketVolume;
        }

        public TimeSpan WindowSize { get; }
        public decimal? PercentageOfMarketVolume { get; }
    }
}
