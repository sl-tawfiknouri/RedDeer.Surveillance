using System;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface ISpoofingRuleParameters : IFilterableRule
    {
        decimal CancellationThreshold { get; }
        decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
        TimeSpan WindowSize { get; }
    }
}