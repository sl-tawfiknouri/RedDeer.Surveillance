using System.Collections.Generic;
using System.Linq;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters
{
    public class RuleParameterDtoIdExtractor : IRuleParameterDtoIdExtractor
    {
        public IReadOnlyCollection<string> ExtractIds(RuleParameterDto dtos)
        {
            if (dtos == null)
            {
                return new string[0];
            }

            var cancelledOrderIds = dtos.CancelledOrders?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var highProfitIds = dtos.HighProfits?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var markingTheCloseIds = dtos.MarkingTheCloses?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var spoofingIds = dtos.Spoofings?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var layeringIds = dtos.Layerings?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var highVolumeIds = dtos.HighVolumes?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var washTradeIds = dtos.WashTrades?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var rampingIds = dtos.Rampings?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var placingOrderWithNoIntentionToExecute = dtos.PlacingOrders?.Select(_ => _.Id)?.ToList() ?? new List<string>();

            var fixedIncomeHighProfitIds = dtos.FixedIncomeHighProfits?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var fixedIncomeHighVolumeIssuanceIds = dtos.FixedIncomeHighVolumeIssuance?.Select(_ => _.Id)?.ToList() ?? new List<string>();
            var fixedIncomeWashTradeIds = dtos.FixedIncomeWashTrades?.Select(_ => _.Id)?.ToList() ?? new List<string>();

            var response = new List<string>();

            response.AddRange(cancelledOrderIds);
            response.AddRange(highProfitIds);
            response.AddRange(markingTheCloseIds);
            response.AddRange(spoofingIds);
            response.AddRange(layeringIds);
            response.AddRange(highVolumeIds);
            response.AddRange(washTradeIds);
            response.AddRange(rampingIds);
            response.AddRange(placingOrderWithNoIntentionToExecute);

            response.AddRange(fixedIncomeHighProfitIds);
            response.AddRange(fixedIncomeHighVolumeIssuanceIds);
            response.AddRange(fixedIncomeWashTradeIds);

            return response;
        }
    }
}
