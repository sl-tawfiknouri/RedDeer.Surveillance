namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SharedKernel.Contracts.Markets;

    public interface IBmllDataRequestManager
    {
        Task Submit(string systemOperationId, IReadOnlyCollection<MarketDataRequest> bmllRequests);
    }
}