using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface ICancelledOrderRuleParameters : IFilterableRule
    {
        TimeSpan WindowSize { get; }
        decimal? CancelledOrderPercentagePositionThreshold { get; }
        decimal? CancelledOrderCountPercentageThreshold { get; }
        int MinimumNumberOfTradesToApplyRuleTo { get; }
        int? MaximumNumberOfTradesToApplyRuleTo { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        bool AggregateNonFactorableIntoOwnCategory { get; set; }

    }
}
