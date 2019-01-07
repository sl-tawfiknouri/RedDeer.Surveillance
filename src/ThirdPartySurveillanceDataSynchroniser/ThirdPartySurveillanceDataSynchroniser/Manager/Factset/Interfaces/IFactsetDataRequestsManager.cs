using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Factset.Interfaces
{
    public interface IFactsetDataRequestsManager
    {
        Task Submit(List<MarketDataRequestDataSource> factsetRequests);
    }
}