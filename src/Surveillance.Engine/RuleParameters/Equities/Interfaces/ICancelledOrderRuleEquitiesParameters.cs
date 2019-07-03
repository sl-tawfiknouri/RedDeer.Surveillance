using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface ICancelledOrderRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable, IReferenceDataFilterable, IMarketCapFilterable
    {
        TimeWindows Windows { get; }
        decimal? CancelledOrderPercentagePositionThreshold { get; }
        decimal? CancelledOrderCountPercentageThreshold { get; }
        int MinimumNumberOfTradesToApplyRuleTo { get; }
        int? MaximumNumberOfTradesToApplyRuleTo { get; }
    }
}
