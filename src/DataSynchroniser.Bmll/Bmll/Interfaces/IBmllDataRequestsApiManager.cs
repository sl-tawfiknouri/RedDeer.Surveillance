namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SharedKernel.Contracts.Markets;

    public interface IBmllDataRequestsApiManager
    {
        Task<SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>> Send(
            List<MarketDataRequest> bmllRequests,
            bool completeWithFailures);
    }
}