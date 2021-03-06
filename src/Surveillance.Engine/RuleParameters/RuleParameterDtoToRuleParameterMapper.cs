﻿namespace Surveillance.Engine.Rules.RuleParameters
{
    using System;
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

    public class RuleParameterDtoToRuleParameterMapper : IRuleParameterToRulesMapper
    {
        private readonly IDecimalRangeRuleFilterProjector _decimalRangeRuleFilterProjector;

        private readonly ILogger<RuleParameterDtoToRuleParameterMapper> _logger;

        private readonly IClientOrganisationalFactorMapper _organisationalFactorMapper;

        private readonly IRuleFilterProjector _ruleFilterProjector;

        public RuleParameterDtoToRuleParameterMapper(
            IRuleFilterProjector ruleFilterProjector,
            IDecimalRangeRuleFilterProjector decimalRangeRuleFilterProjector,
            IClientOrganisationalFactorMapper organisationalFactorMapper,
            ILogger<RuleParameterDtoToRuleParameterMapper> logger)
        {
            this._ruleFilterProjector = ruleFilterProjector ?? throw new ArgumentNullException(nameof(ruleFilterProjector));
            this._decimalRangeRuleFilterProjector = decimalRangeRuleFilterProjector
                                                    ?? throw new ArgumentNullException(
                                                        nameof(decimalRangeRuleFilterProjector));
            this._organisationalFactorMapper = organisationalFactorMapper
                                               ?? throw new ArgumentNullException(nameof(organisationalFactorMapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<ISpoofingRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<SpoofingRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation("asked to map null or empty spoofing dtos");
                return null;
            }

            return dtos.Select(
                _ => new SpoofingRuleEquitiesParameters(
                    _.Id,
                    _.WindowSize,
                    _.CancellationThreshold,
                    _.RelativeSizeMultipleForSpoofExceedingReal,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<CancelledOrderRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation("asked to map null or empty cancelled order dtos");
                return null;
            }

            return dtos.Select(
                _ => new CancelledOrderRuleEquitiesParameters(
                    _.Id,
                    _.WindowSize,
                    _.CancelledOrderPercentagePositionThreshold,
                    _.CancelledOrderCountPercentageThreshold,
                    _.MinimumNumberOfTradesToApplyRuleTo,
                    _.MaximumNumberOfTradesToApplyRuleTo,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<HighProfitsRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation("asked to map null or empty high profits dtos");
                return null;
            }

            return dtos.Select(
                _ => new HighProfitsRuleEquitiesParameters(
                    _.Id,
                    _.WindowSize,
                    _.ForwardWindow,
                    _.PerformHighProfitWindowAnalysis,
                    _.PerformHighProfitDailyAnalysis,
                    _.HighProfitPercentageThreshold,
                    _.HighProfitAbsoluteThreshold,
                    _.UseCurrencyConversions,
                    _.HighProfitCurrencyConversionTargetCurrency,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> Map(
            ScheduledExecution execution,
            List<MarkingTheCloseRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation("asked to map null or empty marking the close dtos");
                return null;
            }

            return dtos.Select(
                _ => new MarkingTheCloseEquitiesParameters(
                    _.Id,
                    _.WindowSize,
                    _.PercentageThresholdDailyVolume,
                    _.PercentageThresholdWindowVolume,
                    _.PercentThresholdOffTouch,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<ILayeringRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<LayeringRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation("asked to map null or empty layering dtos");
                return null;
            }

            return dtos.Select(
                _ => new LayeringRuleEquitiesParameters(
                    _.Id,
                    _.WindowSize,
                    _.PercentageOfMarketDailyVolume,
                    _.PercentageOfMarketWindowVolume,
                    _.CheckForCorrespondingPriceMovement,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<HighVolumeRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation("asked to map null or empty high volume dtos");
                return null;
            }

            return dtos.Select(
                _ => new HighVolumeRuleEquitiesParameters(
                    _.Id,
                    _.WindowSize,
                    _.HighVolumePercentageDaily,
                    _.HighVolumePercentageWindow,
                    _.HighVolumePercentageMarketCap,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IWashTradeRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<WashTradeRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation("asked to map null or empty wash trade dtos");
                return null;
            }

            return dtos.Select(
                _ => new WashTradeRuleEquitiesParameters(
                    _.Id,
                    _.WindowSize,
                    _.PerformAveragePositionAnalysis,
                    _.PerformClusteringPositionAnalysis,
                    _.AveragePositionMinimumNumberOfTrades,
                    _.AveragePositionMaximumPositionValueChange,
                    _.AveragePositionMaximumAbsoluteValueChangeAmount,
                    _.AveragePositionMaximumAbsoluteValueChangeCurrency,
                    _.ClusteringPositionMinimumNumberOfTrades,
                    _.ClusteringPercentageValueDifferenceThreshold,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IRampingRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<RampingRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation("asked to map null or empty ramping dtos");
                return null;
            }

            return dtos.Select(
                _ => new RampingRuleEquitiesParameters(
                    _.Id,
                    _.WindowSize,
                    _.AutoCorrelationCoefficient,
                    _.ThresholdOrdersExecutedInWindow,
                    _.ThresholdVolumePercentageWindow,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IWashTradeRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeWashTradeRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation(
                    $"asked to map null or empty {nameof(FixedIncomeWashTradeRuleParameterDto)}");
                return null;
            }

            return dtos.Select(
                _ => new WashTradeRuleFixedIncomeParameters(
                    _.Id,
                    _.WindowSize,
                    _.PerformAveragePositionAnalysis,
                    _.PerformClusteringPositionAnalysis,
                    _.AveragePositionMinimumNumberOfTrades,
                    _.AveragePositionMaximumPositionValueChange,
                    _.AveragePositionMaximumAbsoluteValueChangeAmount,
                    _.AveragePositionMaximumAbsoluteValueChangeCurrency,
                    _.ClusteringPositionMinimumNumberOfTrades,
                    _.ClusteringPercentageValueDifferenceThreshold,
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IHighProfitsRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeHighProfitRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation(
                    $"asked to map null or empty {nameof(FixedIncomeHighProfitRuleParameterDto)}");
                return null;
            }

            return dtos.Select(
                _ => new HighProfitsRuleFixedIncomeParameters(
                    _.Id,
                    _.WindowSize,
                    _.ForwardWindow,
                    _.PerformHighProfitWindowAnalysis,
                    _.PerformHighProfitDailyAnalysis,
                    _.HighProfitPercentageThreshold,
                    _.HighProfitAbsoluteThreshold,
                    _.UseCurrencyConversions,
                    _.HighProfitCurrencyConversionTargetCurrency,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IHighVolumeIssuanceRuleFixedIncomeParameters> Map(
            ScheduledExecution execution,
            List<FixedIncomeHighVolumeRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation(
                    $"asked to map null or empty {nameof(FixedIncomeHighVolumeRuleParameterDto)}");
                return null;
            }

            return dtos.Select(
                _ => new HighVolumeIssuanceRuleFixedIncomeParameters(
                    _.Id,
                    _.WindowSize,
                    _.PerformDailyAnalysis ? _.HighVolumePercentageDaily : null,
                     _.PerformWindowAnalysis ? _.HighVolumePercentageWindow : null,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    _.PerformTuning)).ToList();
        }

        public IReadOnlyCollection<IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters> Map(
            ScheduledExecution execution,
            List<PlacingOrdersWithNoIntentToExecuteRuleParameterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                this._logger.LogInformation(
                    $"asked to map null or empty {nameof(PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters)}");
                return null;
            }

            return dtos.Select(
                _ => new PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters(
                    _.Id,
                    _.Sigma,
                    _.WindowSize,
                    this._organisationalFactorMapper.Map(_.OrganisationalFactors),
                    _.AggregateNonFactorableIntoOwnCategory,
                    this._decimalRangeRuleFilterProjector.Project(_.MarketCap),
                    this._decimalRangeRuleFilterProjector.Project(_.Turnover),
                    this._ruleFilterProjector.Project(_.Accounts),
                    this._ruleFilterProjector.Project(_.Traders),
                    this._ruleFilterProjector.Project(_.Markets),
                    this._ruleFilterProjector.Project(_.Funds),
                    this._ruleFilterProjector.Project(_.Strategies),
                    this._ruleFilterProjector.Project(_.Sectors),
                    this._ruleFilterProjector.Project(_.Industries),
                    this._ruleFilterProjector.Project(_.Regions),
                    this._ruleFilterProjector.Project(_.Countries),
                    _.PerformTuning)).ToList();
        }
    }
}