using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IHighVolumeRuleParameters : IFilterableRule, IRuleParameter
    {
        decimal? HighVolumePercentageDaily { get; }
        decimal? HighVolumePercentageWindow { get; }
        decimal? HighVolumePercentageMarketCap { get; }
        TimeSpan WindowSize { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; }
        bool AggregateNonFactorableIntoOwnCategory { get; }
    }
}