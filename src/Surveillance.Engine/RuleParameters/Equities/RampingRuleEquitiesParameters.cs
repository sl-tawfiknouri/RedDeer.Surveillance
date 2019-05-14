﻿using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    public class RampingRuleEquitiesParameters : IRampingRuleEquitiesParameters
    {
        public RampingRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal autoCorrelationCoefficient,
            int? thresholdOrdersExecutedInWindow,
            decimal? thresholdVolumePercentageWindow,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategy,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id;
            WindowSize = windowSize;

            AutoCorrelationCoefficient = autoCorrelationCoefficient;
            ThresholdOrdersExecutedInWindow = thresholdOrdersExecutedInWindow;
            ThresholdVolumePercentageWindow = thresholdVolumePercentageWindow;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategy ?? RuleFilter.None();

            Factors = factors;
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public RampingRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal autoCorrelationCoefficient,
            int? thresholdOrdersExecutedInWindow,
            decimal? thresholdVolumePercentageWindow,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id;
            WindowSize = windowSize;

            AutoCorrelationCoefficient = autoCorrelationCoefficient;
            ThresholdOrdersExecutedInWindow = thresholdOrdersExecutedInWindow;
            ThresholdVolumePercentageWindow = thresholdVolumePercentageWindow;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Factors = factors;
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }


        public string Id { get; }

        public TimeSpan WindowSize { get; }
        public decimal AutoCorrelationCoefficient { get; }
        public int? ThresholdOrdersExecutedInWindow { get; }
        public decimal? ThresholdVolumePercentageWindow { get; }

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
    }
}