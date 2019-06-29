using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    [Serializable]
    public class CancelledOrderRuleEquitiesParameters : ICancelledOrderRuleEquitiesParameters
    {
        public CancelledOrderRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? cancelledOrderPositionPercentageThreshold,
            decimal? cancelledOrderCountPercentageThreshold,
            int minimumNumberOfTradesToApplyRuleTo,
            int? maximumNumberOfTradesToApplyRuleTo,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategy,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(windowSize);
            CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategy ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public CancelledOrderRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? cancelledOrderPositionPercentageThreshold,
            decimal? cancelledOrderCountPercentageThreshold,
            int minimumNumberOfTradesToApplyRuleTo,
            int? maximumNumberOfTradesToApplyRuleTo,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;
            Windows = new TimeWindows(windowSize);
            CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        [TuneableIdParameter]
        public string Id { get; set; }
        [TuneableTimespanParameter]
        public TimeSpan WindowSize { get; set; }
        [TuneableDecimalParameter]
        public decimal? CancelledOrderPercentagePositionThreshold { get; set; }
        [TuneableDecimalParameter]
        public decimal? CancelledOrderCountPercentageThreshold { get; set; }
        [TuneableIntegerParameter]
        public int MinimumNumberOfTradesToApplyRuleTo { get; set; }
        [TuneableIntegerParameter]
        public int? MaximumNumberOfTradesToApplyRuleTo { get; set; }
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
                   && (CancelledOrderPercentagePositionThreshold == null
                       || (CancelledOrderPercentagePositionThreshold.Value <= 1
                           && CancelledOrderPercentagePositionThreshold.Value >= 0))
                   && (CancelledOrderCountPercentageThreshold == null
                       || (CancelledOrderCountPercentageThreshold.Value >= 0
                           && CancelledOrderCountPercentageThreshold <= 1))
                && MinimumNumberOfTradesToApplyRuleTo >= 2
                && (MaximumNumberOfTradesToApplyRuleTo == null
                    || MaximumNumberOfTradesToApplyRuleTo.GetValueOrDefault() >= MinimumNumberOfTradesToApplyRuleTo);
        }

        public override int GetHashCode()
        {
            return WindowSize.GetHashCode()
                   * CancelledOrderPercentagePositionThreshold.GetHashCode()
                   * CancelledOrderCountPercentageThreshold.GetHashCode()
                   * MinimumNumberOfTradesToApplyRuleTo.GetHashCode()
                   * MaximumNumberOfTradesToApplyRuleTo.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as CancelledOrderRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return
                this.WindowSize == castObj.WindowSize
                && this.CancelledOrderPercentagePositionThreshold == castObj.CancelledOrderPercentagePositionThreshold
                && this.CancelledOrderCountPercentageThreshold == castObj.CancelledOrderCountPercentageThreshold
                && this.MinimumNumberOfTradesToApplyRuleTo == castObj.MinimumNumberOfTradesToApplyRuleTo
                && this.MaximumNumberOfTradesToApplyRuleTo == castObj.MaximumNumberOfTradesToApplyRuleTo;
        }
    }
}