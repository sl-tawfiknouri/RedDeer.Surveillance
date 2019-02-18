using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;

namespace DataSynchroniser.Api.Factset.Factset.Interfaces
{
    public interface IFactsetDataRequestsManager
    {
        Task Submit(List<MarketDataRequest> factsetRequests);
    }
}