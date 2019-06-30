using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    [Serializable]
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

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        [TuneableIdParameter]
        public string Id { get; set; }

        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }

        /// <summary>
        /// A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? PercentageThresholdDailyVolume { get; set; }

        /// <summary>
        /// A fractional percentage e.g. 0.2 = 20%
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? PercentageThresholdWindowVolume { get; set; }

        /// <summary>
        /// A fractional percentage for how far from touch e.g. % away from bid for a buy; % away from ask for a sell
        /// </summary>
        [TuneableDecimalParameter]
        public decimal? PercentThresholdOffTouch { get; set; }

        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }
        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool HasFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None
                || Funds?.Type != RuleFilterType.None
                || Strategies?.Type != RuleFilterType.None;
        }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && (PercentageThresholdDailyVolume == null
                    || (PercentageThresholdDailyVolume.GetValueOrDefault() >= 0
                        && PercentageThresholdDailyVolume.GetValueOrDefault() <= 1))
               && (PercentageThresholdWindowVolume == null
                   || (PercentageThresholdWindowVolume.GetValueOrDefault() >= 0
                       && PercentageThresholdWindowVolume.GetValueOrDefault() <= 1))
               && (PercentThresholdOffTouch == null
                   || (PercentThresholdOffTouch.GetValueOrDefault() >= 0
                       && PercentThresholdOffTouch.GetValueOrDefault() <= 1));
        }

        public override int GetHashCode()
        {
            return 
                Windows.GetHashCode()
                    * PercentageThresholdDailyVolume.GetHashCode()
                    * PercentageThresholdWindowVolume.GetHashCode()
                    * PercentThresholdOffTouch.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as MarkingTheCloseEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return
                Windows == castObj.Windows
                && PercentageThresholdDailyVolume == castObj.PercentageThresholdDailyVolume
                && PercentageThresholdWindowVolume == castObj.PercentageThresholdWindowVolume
                && PercentThresholdOffTouch == castObj.PercentThresholdOffTouch;
        }

        public bool IsTuned { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }
    }
}
