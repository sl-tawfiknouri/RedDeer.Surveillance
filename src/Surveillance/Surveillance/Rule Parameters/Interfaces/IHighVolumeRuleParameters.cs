using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface IHighVolumeRuleParameters : IFilterableRule
    {
        decimal? HighVolumePercentageDaily { get; }
        decimal? HighVolumePercentageWindow { get; }
        TimeSpan WindowSize { get; }
    }
}