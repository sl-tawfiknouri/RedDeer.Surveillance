using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;

namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    public interface IBmllDataRequestManager
    {
        Task Submit(string systemOperationId, List<MarketDataRequest> bmllRequests);
    }
}
