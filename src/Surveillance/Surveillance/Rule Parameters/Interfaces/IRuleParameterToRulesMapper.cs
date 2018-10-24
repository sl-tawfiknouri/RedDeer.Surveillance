using System.Collections.Generic;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Rules.MarkingTheClose.Interfaces;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface IRuleParameterToRulesMapper
    {
        IReadOnlyCollection<ISpoofingRuleParameters> Map(List<SpoofingRuleParameterDto> dtos);
        IReadOnlyCollection<ICancelledOrderRuleParameters> Map(List<CancelledOrderRuleParameterDto> dtos);
        IReadOnlyCollection<IHighProfitsRuleParameters> Map(List<HighProfitsRuleParameterDto> dtos);
        IReadOnlyCollection<IMarkingTheCloseParameters> Map(List<MarkingTheCloseRuleParameterDto> dtos);
        IReadOnlyCollection<ILayeringRuleParameters> Map(List<LayeringRuleParameterDto> dtos);
        IReadOnlyCollection<IHighVolumeRuleParameters> Map(List<HighVolumeRuleParameterDto> dtos);
    }
}