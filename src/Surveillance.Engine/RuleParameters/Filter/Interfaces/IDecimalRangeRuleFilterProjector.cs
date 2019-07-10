using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces
{
    public interface IDecimalRangeRuleFilterProjector
    {
        DecimalRangeRuleFilter Project(Range filter);
    }
}
