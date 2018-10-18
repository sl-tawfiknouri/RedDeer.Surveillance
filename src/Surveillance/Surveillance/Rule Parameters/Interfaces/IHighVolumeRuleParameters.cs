using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface IHighVolumeRuleParameters
    {
        decimal? HighVolumePercentageDaily { get; }
        decimal? HighVolumePercentageWindow { get; }
        TimeSpan WindowSize { get; }
    }
}