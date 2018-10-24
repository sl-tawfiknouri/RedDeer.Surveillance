using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface ISpoofingRuleParameters : IFilterableRule
    {
        decimal CancellationThreshold { get; }
        decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
        TimeSpan WindowSize { get; }
    }
}