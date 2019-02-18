using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
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