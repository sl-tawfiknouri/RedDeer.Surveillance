using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface ILayeringRuleParameters : IFilterableRule, IRuleParameter
    {
        TimeSpan WindowSize { get; }
        decimal? PercentageOfMarketDailyVolume { get; }
        decimal? PercentageOfMarketWindowVolume { get; }
        bool? CheckForCorrespondingPriceMovement { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; }
        bool AggregateNonFactorableIntoOwnCategory { get; }
    }
}