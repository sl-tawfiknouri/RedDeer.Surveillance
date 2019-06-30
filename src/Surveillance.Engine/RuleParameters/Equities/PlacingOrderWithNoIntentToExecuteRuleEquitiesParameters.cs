using System;
using System.Collections.Generic;
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
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Sigma = sigma;
            Windows = new TimeWindows(windowSize);
            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();
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
            RuleFilter strategies)
        {
            Id = id ?? string.Empty;

            Sigma = sigma;
            Windows = new TimeWindows(windowSize);
            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategies ?? RuleFilter.None();
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

        public bool IsTuned { get; set; }
    }
}
