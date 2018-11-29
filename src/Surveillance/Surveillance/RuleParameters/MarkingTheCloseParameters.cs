using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.Filter;
using Surveillance.RuleParameters.OrganisationalFactors;
using Surveillance.Rules.MarkingTheClose.Interfaces;

namespace Surveillance.RuleParameters
{
    public class MarkingTheCloseParameters : IMarkingTheCloseParameters
    {
        public MarkingTheCloseParameters(
            TimeSpan window,
            decimal? percentageThresholdDailyVolume,
            decimal? percentageThresholdWindowVolume,
            decimal? percentThresholdOffTouch,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Window = window;
            PercentageThresholdDailyVolume = percentageThresholdDailyVolume;
            PercentageThresholdWindowVolume = percentageThresholdWindowVolume;
            PercentThresholdOffTouch = percentThresholdOffTouch;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public MarkingTheCloseParameters(
            TimeSpan window,
            decimal? percentageThresholdDailyVolume,
            decimal? percentageThresholdWindowVolume,
            decimal? percentThresholdOffTouch,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Window = window;
            PercentageThresholdDailyVolume = percentageThresholdDailyVolume;
            PercentageThresholdWindowVolume = percentageThresholdWindowVolume;
            PercentThresholdOffTouch = percentThresholdOffTouch;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public TimeSpan Window { get; }

        /// <summary>
        /// A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        public decimal? PercentageThresholdDailyVolume { get; }

        /// <summary>
        /// A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        public decimal? PercentageThresholdWindowVolume { get; }

        /// <summary>
        /// A fractional percentage for how far from touch e.g. % away from bid for a buy; % away from ask for a sell
        /// </summary>
        public decimal? PercentThresholdOffTouch { get; }

        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool HasFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None;
        }
    }
}
