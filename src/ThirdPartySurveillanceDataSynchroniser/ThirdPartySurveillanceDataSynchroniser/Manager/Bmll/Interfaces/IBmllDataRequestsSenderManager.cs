using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IBmllDataRequestsSenderManager
    {
        Task<SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>> Send(List<MarketDataRequestDataSource> bmllRequests, bool completeWithFailures);
    }
}