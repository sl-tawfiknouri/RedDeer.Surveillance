using System;
using System.Collections.Generic;
using System.Linq;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Services.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Services
{
    public class RuleParameterLeadingTimespanService : IRuleParameterLeadingTimespanService
    {
        public TimeSpan LeadingTimespan(RuleParameterDto dto)
        {
            if (dto == null)
            {
                return TimeSpan.Zero;
            }

            var cancelledOrders = dto.CancelledOrders?.Select(x => x as IIdentifiableRule)?.ToList() ?? new List<IIdentifiableRule>();
            var highProfit = dto.HighProfits?.Select(x => x as IIdentifiableRule)?.ToList() ?? new List<IIdentifiableRule>();
            var markingTheClose = dto.MarkingTheCloses?.Select(x => x as IIdentifiableRule)?.ToList() ?? new List<IIdentifiableRule>();
            var spoofing = dto.Spoofings?.Select(x => x as IIdentifiableRule)?.ToList() ?? new List<IIdentifiableRule>();
            var layering = dto.Layerings?.Select(x => x as IIdentifiableRule)?.ToList() ?? new List<IIdentifiableRule>();
            var highVolume = dto.HighVolumes?.Select(x => x as IIdentifiableRule)?.ToList() ?? new List<IIdentifiableRule>();
            var washTrade = dto.WashTrades?.Select(x => x as IIdentifiableRule)?.ToList() ?? new List<IIdentifiableRule>();

            var identifiableRuleList =
                cancelledOrders
                .Concat(highProfit)
                .Concat(markingTheClose)
                .Concat(spoofing)
                .Concat(layering)
                .Concat(highVolume)
                .Concat(washTrade)
                .Where(x => x != null)
                .ToList();

            if (!identifiableRuleList.Any())
            {
                return TimeSpan.Zero;
            }

            var date = identifiableRuleList.Max(i => i.WindowSize);

            return date;
        }
    }
}
