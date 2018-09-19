using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface ISpoofingRuleParameters
    {
        decimal CancellationThreshold { get; }
        decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
        TimeSpan WindowSize { get; }
    }
}