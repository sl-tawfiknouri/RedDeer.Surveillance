using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface ISpoofingRuleParameters : IFilterableRule, IRuleParameter
    {
        decimal CancellationThreshold { get; }
        decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
        TimeSpan WindowSize { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        bool AggregateNonFactorableIntoOwnCategory { get; set; }
    }
}