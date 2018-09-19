using System;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class SpoofingRuleParameters : ISpoofingRuleParameters
    {
        public SpoofingRuleParameters()
        {
            CancellationThreshold = 0.8m;
            RelativeSizeMultipleForSpoofExceedingReal = 2.5m;
            WindowSize = TimeSpan.FromMinutes(30);
        }

        public TimeSpan WindowSize { get; }
        public decimal CancellationThreshold { get; }
        public decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
    }
}
