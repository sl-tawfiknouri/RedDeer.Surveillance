using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface ILayeringRuleParameters
    {
        TimeSpan WindowSize { get; }
        decimal? PercentageOfMarketDailyVolume { get; }
    }
}