﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;
using Surveillance.Engine.Rules.Mappers.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters
{
    public class RuleParameterDtoToRuleParameterMapper : IRuleParameterToRulesMapper
    {
        private readonly IRuleProjector _ruleProjector;
        private readonly IClientOrganisationalFactorMapper _organisationalFactorMapper;
        private readonly ILogger<RuleParameterDtoToRuleParameterMapper> _logger;

        public RuleParameterDtoToRuleParameterMapper(
            IRuleProjector ruleProjector,
            IClientOrganisationalFactorMapper organisationalFactorMapper,
            ILogger<RuleParameterDtoToRuleParameterMapper> logger)
        {
            _ruleProjector = ruleProjector ?? throw new ArgumentNullException(nameof(ruleProjector));
            _organisationalFactorMapper =
                organisationalFactorMapper
                ?? throw new ArgumentNullException(nameof(organisationalFactorMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<ISpoofingRuleEquitiesParameters> Map(ScheduledExecution execution, List<SpoofingRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty spoofing dtos");
                return null;
            }

            return dtos
                .Select(dto =>
                    new SpoofingRuleEquitiesParameters(
                    dto.Id,
                    dto.WindowSize,
                    dto.CancellationThreshold,
                    dto.RelativeSizeMultipleForSpoofExceedingReal,
                    _ruleProjector.Project(dto.Accounts),
                    _ruleProjector.Project(dto.Traders),
                    _ruleProjector.Project(dto.Markets),
                    _ruleProjector.Project(dto.Funds),
                    _ruleProjector.Project(dto.Strategies),
                    _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                    dto.AggregateNonFactorableIntoOwnCategory,
                    dto.PerformTuning))
                .ToList();
        }

        public IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> Map(ScheduledExecution execution, List<CancelledOrderRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty cancelled order dtos");
                return null;
            }

            return dtos
                .Select(dto =>
                    new CancelledOrderRuleEquitiesParameters(
                        dto.Id,
                        dto.WindowSize,
                        dto.CancelledOrderPercentagePositionThreshold,
                        dto.CancelledOrderCountPercentageThreshold,
                        dto.MinimumNumberOfTradesToApplyRuleTo,
                        dto.MaximumNumberOfTradesToApplyRuleTo,
                        _ruleProjector.Project(dto.Accounts),
                        _ruleProjector.Project(dto.Traders),
                        _ruleProjector.Project(dto.Markets),
                        _ruleProjector.Project(dto.Funds),
                        _ruleProjector.Project(dto.Strategies),
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory,
                        dto.PerformTuning))
                .ToList();
        }

        public IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> Map(ScheduledExecution execution, List<HighProfitsRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty high profits dtos");
                return null;
            }

            return dtos
                .Select(dto =>
                    new HighProfitsRuleEquitiesParameters(
                        dto.Id,
                        dto.WindowSize,
                        dto.ForwardWindow,
                        dto.PerformHighProfitWindowAnalysis,
                        dto.PerformHighProfitDailyAnalysis,
                        dto.HighProfitPercentageThreshold,
                        dto.HighProfitAbsoluteThreshold,
                        dto.UseCurrencyConversions,
                        dto.HighProfitCurrencyConversionTargetCurrency,
                        _ruleProjector.Project(dto.Accounts),
                        _ruleProjector.Project(dto.Traders),
                        _ruleProjector.Project(dto.Markets),
                        _ruleProjector.Project(dto.Funds),
                        _ruleProjector.Project(dto.Strategies),
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory,
                        dto.PerformTuning))
                .ToList();
        }

        public IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> Map(ScheduledExecution execution, List<MarkingTheCloseRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty marking the close dtos");
                return null;
            }

            return dtos
                .Select(dto =>
                    new MarkingTheCloseEquitiesParameters(
                        dto.Id,
                        dto.WindowSize,
                        dto.PercentageThresholdDailyVolume,
                        dto.PercentageThresholdWindowVolume,
                        dto.PercentThresholdOffTouch,
                        _ruleProjector.Project(dto.Accounts),
                        _ruleProjector.Project(dto.Traders),
                        _ruleProjector.Project(dto.Markets),
                        _ruleProjector.Project(dto.Funds),
                        _ruleProjector.Project(dto.Strategies),
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory,
                        dto.PerformTuning))
                .ToList();
        }

        public IReadOnlyCollection<ILayeringRuleEquitiesParameters> Map(ScheduledExecution execution, List<LayeringRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty layering dtos");
                return null;
            }

            return
                dtos
                    .Select(dto =>
                        new LayeringRuleEquitiesParameters(
                            dto.Id,
                            dto.WindowSize,
                            dto.PercentageOfMarketDailyVolume,
                            dto.PercentageOfMarketWindowVolume,
                            dto.CheckForCorrespondingPriceMovement,
                            _ruleProjector.Project(dto.Accounts),
                            _ruleProjector.Project(dto.Traders),
                            _ruleProjector.Project(dto.Markets),
                            _ruleProjector.Project(dto.Funds),
                            _ruleProjector.Project(dto.Strategies),
                            _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                            dto.AggregateNonFactorableIntoOwnCategory,
                            dto.PerformTuning))
                    .ToList();
        }

        public IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> Map(ScheduledExecution execution, List<HighVolumeRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty high volume dtos");
                return null;
            }

            return 
                dtos
                    .Select(dto => 
                        new HighVolumeRuleEquitiesParameters(
                            dto.Id,
                            dto.WindowSize,
                            dto.HighVolumePercentageDaily,
                            dto.HighVolumePercentageWindow,
                            dto.HighVolumePercentageMarketCap,
                            _ruleProjector.Project(dto.Accounts),
                            _ruleProjector.Project(dto.Traders),
                            _ruleProjector.Project(dto.Markets),
                            _ruleProjector.Project(dto.Funds),
                            _ruleProjector.Project(dto.Strategies),
                            _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                            dto.AggregateNonFactorableIntoOwnCategory,
                            dto.PerformTuning))
                    .ToList();

        }

        public IReadOnlyCollection<IWashTradeRuleEquitiesParameters> Map(ScheduledExecution execution, List<WashTradeRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty wash trade dtos");
                return null;
            }

            return
                dtos
                    .Select(dto =>
                        new WashTradeRuleEquitiesParameters(
                            dto.Id,
                            dto.WindowSize,
                            dto.PerformAveragePositionAnalysis,
                            dto.PerformClusteringPositionAnalysis,
                            dto.AveragePositionMinimumNumberOfTrades,
                            dto.AveragePositionMaximumPositionValueChange,
                            dto.AveragePositionMaximumAbsoluteValueChangeAmount,
                            dto.AveragePositionMaximumAbsoluteValueChangeCurrency,
                            dto.ClusteringPositionMinimumNumberOfTrades,
                            dto.ClusteringPercentageValueDifferenceThreshold,
                            _ruleProjector.Project(dto.Accounts),
                            _ruleProjector.Project(dto.Traders),
                            _ruleProjector.Project(dto.Markets),
                            _ruleProjector.Project(dto.Funds),
                            _ruleProjector.Project(dto.Strategies),
                            _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                            dto.AggregateNonFactorableIntoOwnCategory,
                            dto.PerformTuning))
                    .ToList();
        }

        public IReadOnlyCollection<IRampingRuleEquitiesParameters> Map(ScheduledExecution execution, List<RampingRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty ramping dtos");
                return null;
            }

            return
                dtos
                    .Select(dto =>
                        new RampingRuleEquitiesParameters(
                            dto.Id,
                            dto.WindowSize,
                            dto.AutoCorrelationCoefficient,
                            dto.ThresholdOrdersExecutedInWindow,
                            dto.ThresholdVolumePercentageWindow,
                            _ruleProjector.Project(dto.Accounts),
                            _ruleProjector.Project(dto.Traders),
                            _ruleProjector.Project(dto.Markets),
                            _ruleProjector.Project(dto.Funds),
                            _ruleProjector.Project(dto.Strategies),
                            _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                            dto.AggregateNonFactorableIntoOwnCategory,
                            dto.PerformTuning))
                    .ToList();
        }

        public IReadOnlyCollection<IWashTradeRuleFixedIncomeParameters> Map(ScheduledExecution execution, List<FixedIncomeWashTradeRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty {nameof(FixedIncomeWashTradeRuleParameterDto)}");
                return null;
            }

            return dtos
                .Select(dto =>
                    new WashTradeRuleFixedIncomeParameters(
                        dto.Id,
                        dto.WindowSize,
                        dto.PerformAveragePositionAnalysis,
                        dto.PerformClusteringPositionAnalysis,
                        dto.AveragePositionMinimumNumberOfTrades,
                        dto.AveragePositionMaximumPositionValueChange,
                        dto.AveragePositionMaximumAbsoluteValueChangeAmount,
                        dto.AveragePositionMaximumAbsoluteValueChangeCurrency,
                        dto.ClusteringPositionMinimumNumberOfTrades,
                        dto.ClusteringPercentageValueDifferenceThreshold,
                        _ruleProjector.Project(dto.Accounts),
                        _ruleProjector.Project(dto.Traders),
                        _ruleProjector.Project(dto.Markets),
                        _ruleProjector.Project(dto.Funds),
                        _ruleProjector.Project(dto.Strategies),
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory,
                        dto.PerformTuning))
                .ToList();
        }

        public IReadOnlyCollection<IHighProfitsRuleFixedIncomeParameters> Map(ScheduledExecution execution, List<FixedIncomeHighProfitRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty {nameof(FixedIncomeHighProfitRuleParameterDto)}");
                return null;
            }

            return dtos
                .Select(dto =>
                    new HighProfitsRuleFixedIncomeParameters(
                        dto.Id,
                        dto.WindowSize,
                        _ruleProjector.Project(dto.Accounts),
                        _ruleProjector.Project(dto.Traders),
                        _ruleProjector.Project(dto.Markets),
                        _ruleProjector.Project(dto.Funds),
                        _ruleProjector.Project(dto.Strategies),
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory,
                        dto.PerformTuning))
                .ToList();
        }

        public IReadOnlyCollection<IHighVolumeIssuanceRuleFixedIncomeParameters> Map(ScheduledExecution execution, List<FixedIncomeHighVolumeIssuanceRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty {nameof(FixedIncomeHighVolumeIssuanceRuleParameterDto)}");
                return null;
            }

            return dtos
                .Select(dto =>
                    new HighVolumeIssuanceRuleFixedIncomeParameters(
                        dto.Id,
                        dto.WindowSize,
                        _ruleProjector.Project(dto.Accounts),
                        _ruleProjector.Project(dto.Traders),
                        _ruleProjector.Project(dto.Markets),
                        _ruleProjector.Project(dto.Funds),
                        _ruleProjector.Project(dto.Strategies),
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory,
                        dto.PerformTuning))
                .ToList();
        }

        public IReadOnlyCollection<IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<PlacingOrdersWithNoIntentToExecuteRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"asked to map null or empty {nameof(PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters)}");
                return null;
            }

            return dtos
                .Select(_ =>
                    new PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters(
                        _.Id, 
                        _.Sigma, 
                        _.WindowSize, 
                        _organisationalFactorMapper.Map(_.OrganisationalFactors), 
                        _.AggregateNonFactorableIntoOwnCategory, 
                        _ruleProjector.Project(_.Accounts),
                        _ruleProjector.Project(_.Traders),
                        _ruleProjector.Project(_.Markets),
                        _ruleProjector.Project(_.Funds),
                        _ruleProjector.Project(_.Strategies),
                        _.PerformTuning))
                    .ToList();
        }
    }
}
