using System;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class SpoofingRuleParameters : ISpoofingRuleParameters
    {
        public SpoofingRuleParameters(
            TimeSpan windowSize,
            decimal cancellationThreshold,
            decimal relativeSizeMultipleForSpoofingExceedingReal)
        {
            WindowSize = windowSize;
            CancellationThreshold = cancellationThreshold;
            RelativeSizeMultipleForSpoofExceedingReal = relativeSizeMultipleForSpoofingExceedingReal;
        }

        public TimeSpan WindowSize { get; }
        public decimal CancellationThreshold { get; }
        public decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
    }
}
