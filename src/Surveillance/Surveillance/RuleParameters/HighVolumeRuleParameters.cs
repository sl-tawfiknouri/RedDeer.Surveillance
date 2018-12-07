﻿using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.Filter;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters
{
    public class HighVolumeRuleParameters : IHighVolumeRuleParameters
    {
        public HighVolumeRuleParameters(
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow,
            decimal? highVolumePercentageMarketCap,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            WindowSize = windowSize;
            HighVolumePercentageDaily = highVolumePercentageDaily;
            HighVolumePercentageWindow = highVolumePercentageWindow;
            HighVolumePercentageMarketCap = highVolumePercentageMarketCap;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public HighVolumeRuleParameters(
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow,
            decimal? highVolumePercentageMarketCap,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            WindowSize = windowSize;
            HighVolumePercentageDaily = highVolumePercentageDaily;
            HighVolumePercentageWindow = highVolumePercentageWindow;
            HighVolumePercentageMarketCap = highVolumePercentageMarketCap;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public TimeSpan WindowSize { get; }
        public decimal? HighVolumePercentageDaily { get; }
        public decimal? HighVolumePercentageWindow { get; }
        public decimal? HighVolumePercentageMarketCap { get; }
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