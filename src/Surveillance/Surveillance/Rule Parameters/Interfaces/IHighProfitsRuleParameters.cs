﻿using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface IHighProfitsRuleParameters : IFilterableRule
    {
        TimeSpan WindowSize { get; }
        decimal? HighProfitPercentageThreshold { get; }
        decimal? HighProfitAbsoluteThreshold { get; }
        string HighProfitAbsoluteThresholdCurrency { get; }
    }
}