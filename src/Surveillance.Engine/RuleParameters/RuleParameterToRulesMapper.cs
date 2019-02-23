using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using Surveillance.Engine.Rules.Mappers.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters
{
    public class RuleParameterToRulesMapper : IRuleParameterToRulesMapper
    {
        private readonly IRuleProjector _ruleProjector;
        private readonly IClientOrganisationalFactorMapper _organisationalFactorMapper;
        private readonly ILogger<RuleParameterToRulesMapper> _logger;

        public RuleParameterToRulesMapper(
            IRuleProjector ruleProjector,
            IClientOrganisationalFactorMapper organisationalFactorMapper,
            ILogger<RuleParameterToRulesMapper> logger)
        {
            _ruleProjector = ruleProjector ?? throw new ArgumentNullException(nameof(ruleProjector));
            _organisationalFactorMapper =
                organisationalFactorMapper
                ?? throw new ArgumentNullException(nameof(organisationalFactorMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<ISpoofingRuleEquitiesParameters> Map(List<SpoofingRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"RuleParameterToRulesMapper asked to map null or empty spoofing dtos");
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
                    _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                    dto.AggregateNonFactorableIntoOwnCategory))
                .ToList();
        }

        public IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> Map(List<CancelledOrderRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"RuleParameterToRulesMapper asked to map null or empty cancelled order dtos");
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
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory))
                .ToList();
        }

        public IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> Map(List<HighProfitsRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"RuleParameterToRulesMapper asked to map null or empty high profits dtos");
                return null;
            }

            return dtos
                .Select(dto =>
                    new HighProfitsRuleEquitiesParameters(
                        dto.Id,
                        dto.WindowSize,
                        dto.HighProfitPercentageThreshold,
                        dto.HighProfitAbsoluteThreshold,
                        dto.UseCurrencyConversions,
                        dto.HighProfitCurrencyConversionTargetCurrency,
                        _ruleProjector.Project(dto.Accounts),
                        _ruleProjector.Project(dto.Traders),
                        _ruleProjector.Project(dto.Markets),
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory))
                .ToList();
        }

        public IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> Map(List<MarkingTheCloseRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"RuleParameterToRulesMapper asked to map null or empty marking the close dtos");
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
                        _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                        dto.AggregateNonFactorableIntoOwnCategory))
                .ToList();
        }

        public IReadOnlyCollection<ILayeringRuleEquitiesParameters> Map(List<LayeringRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"RuleParameterToRulesMapper asked to map null or empty layering dtos");
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
                            _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                            dto.AggregateNonFactorableIntoOwnCategory))
                    .ToList();
        }

        public IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> Map(List<HighVolumeRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"RuleParameterToRulesMapper asked to map null or empty high volume dtos");
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
                            _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                            dto.AggregateNonFactorableIntoOwnCategory))
                    .ToList();
        }

        public IReadOnlyCollection<IWashTradeRuleEquitiesParameters> Map(List<WashTradeRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                _logger.LogInformation($"RuleParameterToRulesMapper asked to map null or empty wash trade dtos");
                return null;
            }

            return
                dtos
                    .Select(dto =>
                        new WashTradeRuleEquitiesParameters(
                            dto.Id,
                            dto.WindowSize,
                            dto.PerformAveragePositionAnalysis,
                            dto.PerformPairingPositionAnalysis,
                            dto.PerformClusteringPositionAnalysis,
                            dto.AveragePositionMinimumNumberOfTrades,
                            dto.AveragePositionMaximumPositionValueChange,
                            dto.AveragePositionMaximumAbsoluteValueChangeAmount,
                            dto.AveragePositionMaximumAbsoluteValueChangeCurrency,
                            dto.PairingPositionMinimumNumberOfPairedTrades,
                            dto.PairingPositionPercentageValueChangeThresholdPerPair,
                            dto.PairingPositionPercentageVolumeDifferenceThreshold,
                            dto.PairingPositionMaximumAbsoluteValueChangeAmount,
                            dto.PairingPositionMaximumAbsoluteValueChangeCurrency,
                            dto.ClusteringPositionMinimumNumberOfTrades,
                            dto.ClusteringPercentageValueDifferenceThreshold,
                            _ruleProjector.Project(dto.Accounts),
                            _ruleProjector.Project(dto.Traders),
                            _ruleProjector.Project(dto.Markets),
                            _organisationalFactorMapper.Map(dto.OrganisationalFactors),
                            dto.AggregateNonFactorableIntoOwnCategory))
                    .ToList();
        }
    }
}
