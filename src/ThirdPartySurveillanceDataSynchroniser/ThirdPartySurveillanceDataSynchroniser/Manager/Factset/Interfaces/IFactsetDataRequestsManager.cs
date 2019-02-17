using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSynchroniser.Manager.Factset.Interfaces
{
    public interface IFactsetDataRequestsManager
    {
        Task Submit(List<MarketDataRequestDataSource> factsetRequests);
    }
}