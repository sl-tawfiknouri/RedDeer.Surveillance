using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
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
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            WindowSize = windowSize;
            CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategy ?? RuleFilter.None();

            Sectors = sectors ?? RuleFilter.None();
            Industries = industries ?? RuleFilter.None();
            Regions = regions ?? RuleFilter.None();
            Countries = countries ?? RuleFilter.None();

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
            WindowSize = windowSize;
            CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;

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

        public string Id { get; }
        public TimeSpan WindowSize { get; }
        public decimal? CancelledOrderPercentagePositionThreshold { get; }
        public decimal? CancelledOrderCountPercentageThreshold { get; }
        public int MinimumNumberOfTradesToApplyRuleTo { get; }
        public int? MaximumNumberOfTradesToApplyRuleTo { get; }
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