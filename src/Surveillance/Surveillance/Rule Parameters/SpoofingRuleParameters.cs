using System;
using Surveillance.Rule_Parameters.Filter;
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

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
        }

        public SpoofingRuleParameters(
            TimeSpan windowSize,
            decimal cancellationThreshold,
            decimal relativeSizeMultipleForSpoofingExceedingReal,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            WindowSize = windowSize;
            CancellationThreshold = cancellationThreshold;
            RelativeSizeMultipleForSpoofExceedingReal = relativeSizeMultipleForSpoofingExceedingReal;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
        }

        public TimeSpan WindowSize { get; }
        public decimal CancellationThreshold { get; }
        public decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
    }
}
