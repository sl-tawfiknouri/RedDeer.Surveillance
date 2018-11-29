using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface IHighVolumeRuleParameters : IFilterableRule
    {
        decimal? HighVolumePercentageDaily { get; }
        decimal? HighVolumePercentageWindow { get; }
        decimal? HighVolumePercentageMarketCap { get; }
        TimeSpan WindowSize { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; }
        bool AggregateNonFactorableIntoOwnCategory { get; }
    }
}