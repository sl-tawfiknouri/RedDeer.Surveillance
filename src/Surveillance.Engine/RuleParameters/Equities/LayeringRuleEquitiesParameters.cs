using System;
using System.Collections.Generic;
using Domain.Surveillance.Rules.Tuning;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    [Serializable]
    public class LayeringRuleEquitiesParameters : ILayeringRuleEquitiesParameters
    {
        public LayeringRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageOfMarketDailyVolume,
            decimal? percentOfMarketWindowVolume,
            bool? checkForCorrespondingPriceMovement,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(id, windowSize);
            PercentageOfMarketDailyVolume = percentageOfMarketDailyVolume;
            PercentageOfMarketWindowVolume = percentOfMarketWindowVolume;
            CheckForCorrespondingPriceMovement = checkForCorrespondingPriceMovement;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            PerformTuning = performTuning;
        }

        public LayeringRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageOfMarketDailyVolume,
            decimal? percentOfMarketWindowVolume,
            bool? checkForCorrespondingPriceMovement,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(id, windowSize);
            PercentageOfMarketDailyVolume = percentageOfMarketDailyVolume;
            PercentageOfMarketWindowVolume = percentOfMarketWindowVolume;
            CheckForCorrespondingPriceMovement = checkForCorrespondingPriceMovement;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategies ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            PerformTuning = performTuning;
        }

        [TuneableIdParameter]
        public string Id { get; set; }
        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }
        [TuneableDecimalParameter]
        public decimal? PercentageOfMarketDailyVolume { get; set; }
        [TuneableDecimalParameter]
        public decimal? PercentageOfMarketWindowVolume { get; set; }
        [TuneableBoolParameter]
        public bool? CheckForCorrespondingPriceMovement { get; set; }
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
                   && (PercentageOfMarketDailyVolume == null
                       || (PercentageOfMarketDailyVolume.GetValueOrDefault() >= 0
                           && PercentageOfMarketDailyVolume.GetValueOrDefault() <= 1))
                   && (PercentageOfMarketWindowVolume == null
                       || (PercentageOfMarketWindowVolume.GetValueOrDefault() >= 0
                           && PercentageOfMarketWindowVolume.GetValueOrDefault() <= 1));
        }

        public override int GetHashCode()
        {
            return Windows.GetHashCode()
               * PercentageOfMarketDailyVolume.GetHashCode()
               * PercentageOfMarketWindowVolume.GetHashCode()
                * CheckForCorrespondingPriceMovement.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as LayeringRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return this.Windows == castObj.Windows
                   && this.PercentageOfMarketDailyVolume == castObj.PercentageOfMarketDailyVolume
                   && this.PercentageOfMarketWindowVolume == castObj.PercentageOfMarketWindowVolume
                   && this.CheckForCorrespondingPriceMovement == castObj.CheckForCorrespondingPriceMovement;
        }

        public bool PerformTuning { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }
    }
}
