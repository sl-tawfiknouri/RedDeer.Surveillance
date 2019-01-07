using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Factset.Interfaces
{
    public interface IFactsetDataRequestsSenderManager
    {
        Task Send(List<MarketDataRequestDataSource> factsetRequests);
    }
}