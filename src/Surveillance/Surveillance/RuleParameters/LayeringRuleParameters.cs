﻿using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.Filter;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters
{
    public class LayeringRuleParameters : ILayeringRuleParameters
    {
        public LayeringRuleParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageOfMarketDailyVolume,
            decimal? percentOfMarketWindowVolume,
            bool? checkForCorrespondingPriceMovement,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            WindowSize = windowSize;
            PercentageOfMarketDailyVolume = percentageOfMarketDailyVolume;
            PercentageOfMarketWindowVolume = percentOfMarketWindowVolume;
            CheckForCorrespondingPriceMovement = checkForCorrespondingPriceMovement;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public LayeringRuleParameters(
            string id,
            TimeSpan windowSize,
            decimal? percentageOfMarketDailyVolume,
            decimal? percentOfMarketWindowVolume,
            bool? checkForCorrespondingPriceMovement,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            WindowSize = windowSize;
            PercentageOfMarketDailyVolume = percentageOfMarketDailyVolume;
            PercentageOfMarketWindowVolume = percentOfMarketWindowVolume;
            CheckForCorrespondingPriceMovement = checkForCorrespondingPriceMovement;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public string Id { get; }
        public TimeSpan WindowSize { get; }
        public decimal? PercentageOfMarketDailyVolume { get; }
        public decimal? PercentageOfMarketWindowVolume { get; }
        public bool? CheckForCorrespondingPriceMovement { get; }
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
