using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface ICancelledOrderRuleEquitiesParameters : IFilterableRule, IRuleParameter
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
