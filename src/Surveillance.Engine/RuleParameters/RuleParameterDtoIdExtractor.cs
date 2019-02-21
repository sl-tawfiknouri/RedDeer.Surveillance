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

            var cancelledOrderIds = dtos.CancelledOrders?.Select(x => x.Id)?.ToList() ?? new List<string>();
            var highProfitIds = dtos.HighProfits?.Select(x => x.Id)?.ToList() ?? new List<string>();
            var markingTheCloseIds = dtos.MarkingTheCloses?.Select(x => x.Id)?.ToList() ?? new List<string>();
            var spoofingIds = dtos.Spoofings?.Select(x => x.Id)?.ToList() ?? new List<string>();
            var layeringIds = dtos.Layerings?.Select(x => x.Id)?.ToList() ?? new List<string>();
            var highVolumeIds = dtos.HighVolumes?.Select(x => x.Id)?.ToList() ?? new List<string>();
            var washTradeIds = dtos.WashTrades?.Select(x => x.Id)?.ToList() ?? new List<string>();

            var response = new List<string>();

            response.AddRange(cancelledOrderIds);
            response.AddRange(highProfitIds);
            response.AddRange(markingTheCloseIds);
            response.AddRange(spoofingIds);
            response.AddRange(layeringIds);
            response.AddRange(highVolumeIds);
            response.AddRange(washTradeIds);

            return response;
        }
    }
}
