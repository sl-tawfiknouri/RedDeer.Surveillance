using System.Collections.Generic;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IRuleParameterToRulesMapper
    {
        IReadOnlyCollection<ISpoofingRuleParameters> Map(List<SpoofingRuleParameterDto> dtos);
        IReadOnlyCollection<ICancelledOrderRuleParameters> Map(List<CancelledOrderRuleParameterDto> dtos);
        IReadOnlyCollection<IHighProfitsRuleParameters> Map(List<HighProfitsRuleParameterDto> dtos);
        IReadOnlyCollection<IMarkingTheCloseParameters> Map(List<MarkingTheCloseRuleParameterDto> dtos);
        IReadOnlyCollection<ILayeringRuleParameters> Map(List<LayeringRuleParameterDto> dtos);
        IReadOnlyCollection<IHighVolumeRuleParameters> Map(List<HighVolumeRuleParameterDto> dtos);
        IReadOnlyCollection<IWashTradeRuleParameters> Map(List<WashTradeRuleParameterDto> dtos);
    }
}