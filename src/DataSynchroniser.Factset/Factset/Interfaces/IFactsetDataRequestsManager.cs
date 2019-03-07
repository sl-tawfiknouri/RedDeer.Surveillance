using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.Contracts.Markets;

namespace DataSynchroniser.Api.Factset.Factset.Interfaces
{
    public interface IFactsetDataRequestsManager
    {
        Task Submit(IReadOnlyCollection<MarketDataRequest> factsetRequests);
    }
}