﻿using System.Collections.Generic;
using System.Linq;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class RuleParameterToRulesMapper : IRuleParameterToRulesMapper
    {
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
                    dto.RelativeSizeMultipleForSpoofExceedingReal))
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
                        dto.MaximumNumberOfTradesToApplyRuleTo))
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
                        dto.HighProfitAbsoluteThresholdCurrency))
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
                        dto.PercentThresholdOffTouch))
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
                            dto.CheckForCorrespondingPriceMovement))
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
                            dto.HighVolumePercentageWindow))
                    .ToList();
        }
    }
}
