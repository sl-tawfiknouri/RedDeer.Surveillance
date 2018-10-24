using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface ILayeringRuleParameters : IFilterableRule
    {
        TimeSpan WindowSize { get; }
        decimal? PercentageOfMarketDailyVolume { get; }
        decimal? PercentageOfMarketWindowVolume { get; }
        bool? CheckForCorrespondingPriceMovement { get; }
    }
}