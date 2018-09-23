using System;
// ReSharper disable UnusedMember.Global

namespace Surveillance.Rules.Marking_The_Close.Interfaces
{
    public interface IMarkingTheCloseParameters
    {
        decimal? PercentageThresholdDailyVolume { get; }
        decimal? PercentageThresholdWindowVolume { get; }
        decimal? PercentThresholdOffTouch { get; }
        TimeSpan Window { get; }
    }
}