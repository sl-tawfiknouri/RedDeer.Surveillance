﻿using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Rules.Marking_The_Close.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class RuleParameterToRulesMapper : IRuleParameterToRulesMapper
    {
        public ISpoofingRuleParameters Map(SpoofingRuleParameterDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new SpoofingRuleParameters(
                dto.WindowSize,
                dto.CancellationThreshold,
                dto.RelativeSizeMultipleForSpoofExceedingReal);
        }

        public ICancelledOrderRuleParameters Map(CancelledOrderRuleParameterDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new CancelledOrderRuleParameters(
                dto.WindowSize,
                dto.CancelledOrderPercentagePositionThreshold,
                dto.CancelledOrderCountPercentageThreshold,
                dto.MinimumNumberOfTradesToApplyRuleTo,
                dto.MaximumNumberOfTradesToApplyRuleTo);
        }

        public IHighProfitsRuleParameters Map(HighProfitsRuleParameterDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new HighProfitsRuleParameters(
                dto.WindowSize,
                dto.HighProfitPercentageThreshold,
                dto.HighProfitAbsoluteThreshold,
                dto.HighProfitAbsoluteThresholdCurrency);
        }

        public IMarkingTheCloseParameters Map(MarkingTheCloseRuleParameterDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new MarkingTheCloseParameters(
                dto.Window,
                dto.PercentageThresholdDailyVolume,
                dto.PercentageThresholdWindowVolume,
                dto.PercentThresholdOffTouch);
        }
    }
}