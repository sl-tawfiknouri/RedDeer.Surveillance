using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.Filter;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters
{
    public class SpoofingRuleParameters : ISpoofingRuleParameters
    {
        public SpoofingRuleParameters(
            TimeSpan windowSize,
            decimal cancellationThreshold,
            decimal relativeSizeMultipleForSpoofingExceedingReal,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            WindowSize = windowSize;
            CancellationThreshold = cancellationThreshold;
            RelativeSizeMultipleForSpoofExceedingReal = relativeSizeMultipleForSpoofingExceedingReal;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public SpoofingRuleParameters(
            TimeSpan windowSize,
            decimal cancellationThreshold,
            decimal relativeSizeMultipleForSpoofingExceedingReal,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            WindowSize = windowSize;
            CancellationThreshold = cancellationThreshold;
            RelativeSizeMultipleForSpoofExceedingReal = relativeSizeMultipleForSpoofingExceedingReal;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public TimeSpan WindowSize { get; }
        public decimal CancellationThreshold { get; }
        public decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
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
