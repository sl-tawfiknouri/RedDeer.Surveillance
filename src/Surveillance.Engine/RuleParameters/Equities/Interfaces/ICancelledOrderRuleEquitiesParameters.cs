using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface ICancelledOrderRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable
    {
        TimeSpan WindowSize { get; set; }
        decimal? CancelledOrderPercentagePositionThreshold { get; set; }
        decimal? CancelledOrderCountPercentageThreshold { get; set; }
        int MinimumNumberOfTradesToApplyRuleTo { get; set; }
        int? MaximumNumberOfTradesToApplyRuleTo { get; set; }
    }
}
