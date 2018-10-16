using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Rules.Marking_The_Close.Interfaces;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface IRuleParameterToRulesMapper
    {
        ISpoofingRuleParameters Map(SpoofingRuleParameterDto dto);
        ICancelledOrderRuleParameters Map(CancelledOrderRuleParameterDto dto);
        IHighProfitsRuleParameters Map(HighProfitsRuleParameterDto dto);
        IMarkingTheCloseParameters Map(MarkingTheCloseRuleParameterDto dto);
        ILayeringRuleParameters Map(LayeringRuleParameterDto dto);
    }
}