using System.Collections.Generic;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IRuleParameterToRulesMapper
    {
        IReadOnlyCollection<ISpoofingRuleEquitiesParameters> Map(List<SpoofingRuleParameterDto> dtos);
        IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> Map(List<CancelledOrderRuleParameterDto> dtos);
        IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> Map(List<HighProfitsRuleParameterDto> dtos);
        IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> Map(List<MarkingTheCloseRuleParameterDto> dtos);
        IReadOnlyCollection<ILayeringRuleEquitiesParameters> Map(List<LayeringRuleParameterDto> dtos);
        IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> Map(List<HighVolumeRuleParameterDto> dtos);
        IReadOnlyCollection<IWashTradeRuleEquitiesParameters> Map(List<WashTradeRuleParameterDto> dtos);
    }
}