using System;
using System.Collections.Generic;
using Domain.Surveillance.Scheduling;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters
{
    public class RuleParameterToRulesMapperTuningDecorator : IRuleParameterToRulesMapperDecorator
    {
        private readonly IRuleParameterToRulesMapper _mapper;

        public RuleParameterToRulesMapperTuningDecorator(IRuleParameterToRulesMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IReadOnlyCollection<ISpoofingRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<SpoofingRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<CancelledOrderRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<HighProfitsRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> Map(
            ScheduledExecution execution,
            List<MarkingTheCloseRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<ILayeringRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<LayeringRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<HighVolumeRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IWashTradeRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<WashTradeRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IRampingRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<RampingRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<PlacingOrdersWithNoIntentToExecuteRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IWashTradeRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeWashTradeRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IHighProfitsRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeHighProfitRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }

        public IReadOnlyCollection<IHighVolumeIssuanceRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeHighVolumeIssuanceRuleParameterDto> dtos)
        {
            return _mapper.Map(execution, dtos);
        }
    }
}
