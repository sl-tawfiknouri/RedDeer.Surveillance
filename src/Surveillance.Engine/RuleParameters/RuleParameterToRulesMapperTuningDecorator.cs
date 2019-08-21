namespace Surveillance.Engine.Rules.RuleParameters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Newtonsoft.Json;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;

    using Surveillance.DataLayer.Aurora.Tuning;
    using Surveillance.DataLayer.Aurora.Tuning.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Tuning.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

    public class RuleParameterToRulesMapperTuningDecorator : IRuleParameterToRulesMapperDecorator
    {
        private readonly IRuleParameterToRulesMapper _mapper;

        private readonly IRuleParameterTuner _tuner;

        private readonly ITuningRepository _tuningRepository;

        public RuleParameterToRulesMapperTuningDecorator(
            IRuleParameterTuner tuner,
            IRuleParameterToRulesMapper mapper,
            ITuningRepository tuningRepository)
        {
            this._tuner = tuner ?? throw new ArgumentNullException(nameof(tuner));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._tuningRepository = tuningRepository ?? throw new ArgumentNullException(nameof(tuningRepository));
        }

        public IReadOnlyCollection<ISpoofingRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<SpoofingRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<CancelledOrderRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<HighProfitsRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> Map(
            ScheduledExecution execution,
            List<MarkingTheCloseRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<ILayeringRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<LayeringRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<HighVolumeRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IWashTradeRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<WashTradeRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IRampingRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<RampingRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<PlacingOrdersWithNoIntentToExecuteRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IWashTradeRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeWashTradeRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IHighProfitsRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeHighProfitRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }

        public IReadOnlyCollection<IHighVolumeIssuanceRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeHighVolumeIssuanceRuleParameterDto> dtos)
        {
            if (!execution?.IsBackTest ?? true) return this._mapper.Map(execution, dtos);

            var mappedDtos = this._mapper.Map(execution, dtos).ToList();
            var tunedDtos = mappedDtos.Where(_ => _.PerformTuning).SelectMany(this._tuner.ParametersFramework)
                .Where(_ => _.Valid() && _.TunedParam != null).Distinct().ToList();

            var saveableDtos = tunedDtos
                .Select(_ => new TuningRepository.TuningPair(_.TunedParam, JsonConvert.SerializeObject(_))).ToList();

            this._tuningRepository.SaveTasks(saveableDtos).Wait();

            return mappedDtos.Concat(tunedDtos).ToList();
        }
    }
}