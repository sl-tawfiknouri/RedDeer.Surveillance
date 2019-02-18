using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;

namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    public interface IBmllDataRequestsRescheduleManager
    {
        Task RescheduleRuleRun(string systemProcessOperationId, List<MarketDataRequest> bmllRequests);
    }
}