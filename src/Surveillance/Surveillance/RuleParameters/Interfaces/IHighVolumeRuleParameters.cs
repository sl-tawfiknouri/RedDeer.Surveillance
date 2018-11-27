using System;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface IHighVolumeRuleParameters : IFilterableRule
    {
        decimal? HighVolumePercentageDaily { get; }
        decimal? HighVolumePercentageWindow { get; }
        decimal? HighVolumePercentageMarketCap { get; }
        TimeSpan WindowSize { get; }
    }
}