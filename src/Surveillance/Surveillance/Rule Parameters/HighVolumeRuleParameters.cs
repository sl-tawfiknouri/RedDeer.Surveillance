using System;
using Surveillance.Rule_Parameters.Filter;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class HighVolumeRuleParameters : IHighVolumeRuleParameters
    {
        public HighVolumeRuleParameters(
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow)
        {
            WindowSize = windowSize;
            HighVolumePercentageDaily = highVolumePercentageDaily;
            HighVolumePercentageWindow = highVolumePercentageWindow;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
        }

        public HighVolumeRuleParameters(
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            WindowSize = windowSize;
            HighVolumePercentageDaily = highVolumePercentageDaily;
            HighVolumePercentageWindow = highVolumePercentageWindow;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
        }

        public TimeSpan WindowSize { get; }
        public decimal? HighVolumePercentageDaily { get; }
        public decimal? HighVolumePercentageWindow { get; }
        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
    }
}
