using System;
using Surveillance.Rule_Parameters.Filter;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class LayeringRuleParameters : ILayeringRuleParameters
    {
        public LayeringRuleParameters(
            TimeSpan windowSize,
            decimal? percentageOfMarketDailyVolume,
            decimal? percentOfMarketWindowVolume,
            bool? checkForCorrespondingPriceMovement)
        {
            WindowSize = windowSize;
            PercentageOfMarketDailyVolume = percentageOfMarketDailyVolume;
            PercentageOfMarketWindowVolume = percentOfMarketWindowVolume;
            CheckForCorrespondingPriceMovement = checkForCorrespondingPriceMovement;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
        }

        public LayeringRuleParameters(
            TimeSpan windowSize,
            decimal? percentageOfMarketDailyVolume,
            decimal? percentOfMarketWindowVolume,
            bool? checkForCorrespondingPriceMovement,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            WindowSize = windowSize;
            PercentageOfMarketDailyVolume = percentageOfMarketDailyVolume;
            PercentageOfMarketWindowVolume = percentOfMarketWindowVolume;
            CheckForCorrespondingPriceMovement = checkForCorrespondingPriceMovement;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
        }

        public TimeSpan WindowSize { get; }
        public decimal? PercentageOfMarketDailyVolume { get; }
        public decimal? PercentageOfMarketWindowVolume { get; }
        public bool? CheckForCorrespondingPriceMovement { get; }
        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
    }
}
