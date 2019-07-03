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
    public class PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters 
        : IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters
    {
        public PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters(
            string id, 
            decimal sigma,
            TimeSpan windowSize,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            Id = id ?? string.Empty;

            Sigma = sigma;
            Windows = new TimeWindows(id, windowSize);
            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            PerformTuning = performTuning;
            Sectors = RuleFilter.None();
            Industries = RuleFilter.None();
            Regions = RuleFilter.None();
            Countries = RuleFilter.None();
        }

        public PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters(
            string id,
            decimal sigma,
            TimeSpan windowSize,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries,
            bool performTuning)
        {
            Id = id ?? string.Empty;

            Sigma = sigma;
            Windows = new TimeWindows(id, windowSize);
            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategies ?? RuleFilter.None();

            PerformTuning = performTuning;
            Sectors = sectors ?? RuleFilter.None();
            Industries = industries ?? RuleFilter.None();
            Regions = regions ?? RuleFilter.None();
            Countries = countries ?? RuleFilter.None();
        }

        [TuneableIdParameter]
        public string Id { get; set; }
        [TuneableDecimalParameter]
        public decimal Sigma { get; set; }
        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }
        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }

        public RuleFilter Sectors { get; set; }
        public RuleFilter Industries { get; set; }
        public RuleFilter Regions { get; set; }
        public RuleFilter Countries { get; set; }

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

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                   && Sigma >= 0;
        }

        public override int GetHashCode()
        {
            return Windows.GetHashCode()
               * Sigma.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return Windows == castObj.Windows
                   && Sigma == castObj.Sigma;
        }

        public bool PerformTuning { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }
    }
}
