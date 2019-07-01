using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    public class MarkingTheCloseEquitiesParameters : IMarkingTheCloseEquitiesParameters
    {
        public MarkingTheCloseEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageThresholdDailyVolume,
            decimal? percentageThresholdWindowVolume,
            decimal? percentThresholdOffTouch,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(windowSize);
            PercentageThresholdDailyVolume = percentageThresholdDailyVolume;
            PercentageThresholdWindowVolume = percentageThresholdWindowVolume;
            PercentThresholdOffTouch = percentThresholdOffTouch;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Sectors = RuleFilter.None();
            Industries = RuleFilter.None();
            Regions = RuleFilter.None();
            Countries = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public MarkingTheCloseEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageThresholdDailyVolume,
            decimal? percentageThresholdWindowVolume,
            decimal? percentThresholdOffTouch,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(windowSize);
            PercentageThresholdDailyVolume = percentageThresholdDailyVolume;
            PercentageThresholdWindowVolume = percentageThresholdWindowVolume;
            PercentThresholdOffTouch = percentThresholdOffTouch;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategies ?? RuleFilter.None();

            Sectors = sectors ?? RuleFilter.None();
            Industries = industries ?? RuleFilter.None();
            Regions = regions ?? RuleFilter.None();
            Countries = countries ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public string Id { get; }
        public TimeWindows Windows { get; }

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
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }

        public RuleFilter Sectors { get; set; }
        public RuleFilter Industries { get; set; }
        public RuleFilter Regions { get; set; }
        public RuleFilter Countries { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool HasInternalFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None
                || Funds?.Type != RuleFilterType.None
                || Strategies?.Type != RuleFilterType.None;
        }

        public bool HasReferenceDataFilters()
        {
            return
                Sectors?.Type != RuleFilterType.None
                || Industries?.Type != RuleFilterType.None
                || Regions?.Type != RuleFilterType.None
                || Countries?.Type != RuleFilterType.None;
        }
    }
}
