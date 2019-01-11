using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IBmllDataRequestsSenderManager
    {
        Task<IReadOnlyCollection<IGetTimeBarPair>> Send(List<MarketDataRequestDataSource> bmllRequests);
    }
}