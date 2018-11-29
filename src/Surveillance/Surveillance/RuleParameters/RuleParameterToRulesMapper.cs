using System;
using System.Collections.Generic;
using System.Linq;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Mappers.Interfaces;
using Surveillance.RuleParameters.Filter.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.MarkingTheClose.Interfaces;

namespace Surveillance.RuleParameters
{
    public class RuleParameterToRulesMapper : IRuleParameterToRulesMapper
    {
        private readonly IRuleProjector _ruleProjector;
        private readonly IClientOrganisationalFactorMapper _organisationalFactorMapper;

        public RuleParameterToRulesMapper(
            IRuleProjector ruleProjector,
            IClientOrganisationalFactorMapper organisationalFactorMapper)
        {
            _ruleProjector = ruleProjector ?? throw new ArgumentNullException(nameof(ruleProjector));
            _organisationalFactorMapper =
                organisationalFactorMapper
                ?? throw new ArgumentNullException(nameof(organisationalFactorMapper));
        }

        public IReadOnlyCollection<ISpoofingRuleParameters> Map(List<SpoofingRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                return null;
            }

            return dtos
                .Select(dto =>
                    new SpoofingRuleParameters(
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

        public IReadOnlyCollection<ICancelledOrderRuleParameters> Map(List<CancelledOrderRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                return null;
            }

            return dtos
                .Select(dto =>
                    new CancelledOrderRuleParameters(
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

        public IReadOnlyCollection<IHighProfitsRuleParameters> Map(List<HighProfitsRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                return null;
            }

            return dtos
                .Select(dto =>
                    new HighProfitsRuleParameters(
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

        public IReadOnlyCollection<IMarkingTheCloseParameters> Map(List<MarkingTheCloseRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                return null;
            }

            return dtos
                .Select(dto =>
                    new MarkingTheCloseParameters(
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

        public IReadOnlyCollection<ILayeringRuleParameters> Map(List<LayeringRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                return null;
            }

            return
                dtos
                    .Select(dto =>
                        new LayeringRuleParameters(
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

        public IReadOnlyCollection<IHighVolumeRuleParameters> Map(List<HighVolumeRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                return null;
            }

            return 
                dtos
                    .Select(dto => 
                        new HighVolumeRuleParameters(
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

        public IReadOnlyCollection<IWashTradeRuleParameters> Map(List<WashTradeRuleParameterDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                return null;
            }

            return
                dtos
                    .Select(dto =>
                        new WashTradeRuleParameters(
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
