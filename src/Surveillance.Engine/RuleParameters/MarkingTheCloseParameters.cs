using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters
{
    public class MarkingTheCloseParameters : IMarkingTheCloseParameters
    {
        public MarkingTheCloseParameters(
            string id,
            TimeSpan window,
            decimal? percentageThresholdDailyVolume,
            decimal? percentageThresholdWindowVolume,
            decimal? percentThresholdOffTouch,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

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
            string id,
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
            Id = id ?? string.Empty;

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

        public string Id { get; }
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
