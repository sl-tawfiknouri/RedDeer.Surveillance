using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IBmllDataRequestManager
    {
        Task Submit(string systemOperationId, List<MarketDataRequestDataSource> bmllRequests);
    }
}
