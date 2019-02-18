using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;

namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    public interface IBmllDataRequestsSenderManager
    {
        Task<SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>> Send(List<MarketDataRequest> bmllRequests, bool completeWithFailures);
    }
}