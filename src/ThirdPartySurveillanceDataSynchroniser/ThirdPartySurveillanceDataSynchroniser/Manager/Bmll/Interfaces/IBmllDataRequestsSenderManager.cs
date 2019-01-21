using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IBmllDataRequestsSenderManager
    {
        Task<SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>> Send(List<MarketDataRequestDataSource> bmllRequests, bool completeWithFailures);
    }
}