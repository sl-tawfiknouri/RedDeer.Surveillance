using System;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class LayeringRuleParameters : ILayeringRuleParameters
    {
        public LayeringRuleParameters(
            TimeSpan windowSize,
            decimal? percentageOfMarketDailyVolume,
            decimal? percentOfMarketWindowVolume,
            bool? checkForCorrespondingPriceMovement)
        {
            WindowSize = windowSize;
            PercentageOfMarketDailyVolume = percentageOfMarketDailyVolume;
            PercentageOfMarketWindowVolume = percentOfMarketWindowVolume;
            CheckForCorrespondingPriceMovement = checkForCorrespondingPriceMovement;
        }

        public TimeSpan WindowSize { get; }
        public decimal? PercentageOfMarketDailyVolume { get; }
        public decimal? PercentageOfMarketWindowVolume { get; }
        public bool? CheckForCorrespondingPriceMovement { get; }
    }
}
