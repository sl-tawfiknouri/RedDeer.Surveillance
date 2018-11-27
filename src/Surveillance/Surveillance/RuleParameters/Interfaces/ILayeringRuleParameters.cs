using System;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface ILayeringRuleParameters : IFilterableRule
    {
        TimeSpan WindowSize { get; }
        decimal? PercentageOfMarketDailyVolume { get; }
        decimal? PercentageOfMarketWindowVolume { get; }
        bool? CheckForCorrespondingPriceMovement { get; }
    }
}