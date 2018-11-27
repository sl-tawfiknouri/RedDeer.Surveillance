﻿using System;
using Surveillance.RuleParameters.Interfaces;

// ReSharper disable UnusedMember.Global

namespace Surveillance.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseParameters : IFilterableRule
    {
        decimal? PercentageThresholdDailyVolume { get; }
        decimal? PercentageThresholdWindowVolume { get; }
        decimal? PercentThresholdOffTouch { get; }
        TimeSpan Window { get; }
    }
}