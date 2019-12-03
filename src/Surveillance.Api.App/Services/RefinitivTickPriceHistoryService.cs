using Surveillance.Api.App.Types.TickPriceHistory;
using Surveillance.Data.Universe.Refinitiv.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Surveillance.Api.App.Services
{
    [Obsolete("POC HACK")]
    public interface IRefinitivTickPriceHistoryService
    {
        Task<IList<TickPriceHistoryTimeBar>> GetEndOfDayTimeBarsAsync(DateTime? startDateTime, DateTime? endDateTime, List<string> rics);
    }

    [Obsolete("POC HACK")]
    public class RefinitivTickPriceHistoryService
        : IRefinitivTickPriceHistoryService
    {
        private readonly IRefinitivTickPriceHistoryApi refinitivTickPriceHistoryApi; 

        public RefinitivTickPriceHistoryService(
            IRefinitivTickPriceHistoryApi refinitivTickPriceHistoryApi)
        {
            this.refinitivTickPriceHistoryApi = refinitivTickPriceHistoryApi ?? throw new ArgumentNullException(nameof(refinitivTickPriceHistoryApi));
        }

        public async Task<IList<TickPriceHistoryTimeBar>> GetEndOfDayTimeBarsAsync(DateTime? startDateTime, DateTime? endDateTime, List<string> rics)
        {
            var response = await refinitivTickPriceHistoryApi.GetInterdayTimeBars(startDateTime, endDateTime, rics);

            var result =  response.Select(Map).ToList();

            return result;
        }

        private TickPriceHistoryTimeBar Map(Data.Universe.Refinitiv.EndOfDaySecurityTimeBar timebar)
        {
            var result = new TickPriceHistoryTimeBar
            {
                Ric = timebar.SecurityIdentifiers?.Ric,
                CloseAsk = timebar.TimeBar.CloseAsk,
                //Close = r.TimeBar.C
                CurrencyCode = timebar.TimeBar.CurrencyCode,
                EpochUtc = timebar.TimeBar.EpochUtc,
                High = timebar.TimeBar.High,
                HighAsk = timebar.TimeBar.HighAsk,
                Low = timebar.TimeBar.Low
            };

            return result;
        }
    }
}
