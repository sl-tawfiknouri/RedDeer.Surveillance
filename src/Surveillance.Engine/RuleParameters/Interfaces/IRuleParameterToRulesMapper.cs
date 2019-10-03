namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;

    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

    public interface IRuleParameterToRulesMapper
    {
        IReadOnlyCollection<ISpoofingRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<SpoofingRuleParameterDto> dtos);

        IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<CancelledOrderRuleParameterDto> dtos);

        IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<HighProfitsRuleParameterDto> dtos);

        IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> Map(
            ScheduledExecution execution,
            List<MarkingTheCloseRuleParameterDto> dtos);

        IReadOnlyCollection<ILayeringRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<LayeringRuleParameterDto> dtos);

        IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<HighVolumeRuleParameterDto> dtos);

        IReadOnlyCollection<IWashTradeRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<WashTradeRuleParameterDto> dtos);

        IReadOnlyCollection<IRampingRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<RampingRuleParameterDto> dtos);

        IReadOnlyCollection<IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<PlacingOrdersWithNoIntentToExecuteRuleParameterDto> dtos);

        IReadOnlyCollection<IWashTradeRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeWashTradeRuleParameterDto> dtos);

        IReadOnlyCollection<IHighProfitsRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeHighProfitRuleParameterDto> dtos);

        IReadOnlyCollection<IHighVolumeIssuanceRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeHighVolumeRuleParameterDto> dtos);
    }
}