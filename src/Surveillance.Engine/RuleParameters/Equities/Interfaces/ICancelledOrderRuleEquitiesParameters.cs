using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface ICancelledOrderRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable, IReferenceDataFilterable, IMarketCapFilterable
    {
        TimeWindows Windows { get; set; }
        decimal? CancelledOrderPercentagePositionThreshold { get; set; }
        decimal? CancelledOrderCountPercentageThreshold { get; set; }
        int MinimumNumberOfTradesToApplyRuleTo { get; set; }
        int? MaximumNumberOfTradesToApplyRuleTo { get; set; }
    }
}
