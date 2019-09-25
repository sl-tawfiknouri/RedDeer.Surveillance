namespace Surveillance.Engine.Rules.RuleParameters.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.Services.Interfaces;

    public class RuleParameterAdjustedTimespanService : IRuleParameterAdjustedTimespanService
    {
        public TimeSpan LeadingTimespan(RuleParameterDto dto)
        {
            if (dto == null)
            {
                return TimeSpan.Zero;
            }

            var cancelledOrders = dto.CancelledOrders?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var highProfit = dto.HighProfits?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var markingTheClose = dto.MarkingTheCloses?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var spoofing = dto.Spoofings?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var layering = dto.Layerings?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var highVolume = dto.HighVolumes?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var washTrade = dto.WashTrades?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var ramping = dto.Rampings?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var placingOrders = dto.PlacingOrders?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var fixedIncomeWashTrades = dto.FixedIncomeWashTrades?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var fixedIncomeHighProfit = dto.FixedIncomeHighProfits?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();
            var fixedIncomeHighVolumeIssuance = dto.FixedIncomeHighVolumeIssuance?.OfType<IIdentifiableRule>().ToList() ?? new List<IIdentifiableRule>();

            var identifiableRuleList = 
                cancelledOrders
                    .Concat(highProfit)
                    .Concat(markingTheClose)
                    .Concat(spoofing)
                    .Concat(layering)
                    .Concat(highVolume)
                    .Concat(washTrade)
                    .Concat(ramping)
                    .Concat(placingOrders)
                    .Concat(fixedIncomeHighProfit)
                    .Concat(fixedIncomeWashTrades)
                    .Concat(fixedIncomeHighVolumeIssuance)
                    .Where(_ => _ != null)
                    .ToList();

            if (!identifiableRuleList.Any())
            {
                return TimeSpan.Zero;
            }

            var date = identifiableRuleList.Max(i => i.WindowSize);

            return date;
        }

        public TimeSpan TrailingTimeSpan(RuleParameterDto dto)
        {
            var forwardWindows = dto?.HighProfits?.Select(_ => _.ForwardWindow)?.ToList() ?? new List<TimeSpan>();

            if (!forwardWindows.Any())
            {
                return TimeSpan.Zero;
            }

            return forwardWindows.Max(_ => _);
        }
    }
}