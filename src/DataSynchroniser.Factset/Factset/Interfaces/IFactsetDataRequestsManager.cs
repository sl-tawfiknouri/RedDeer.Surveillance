namespace DataSynchroniser.Api.Factset.Factset.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SharedKernel.Contracts.Markets;

    public interface IFactsetDataRequestsManager
    {
        Task Submit(IReadOnlyCollection<MarketDataRequest> factsetRequests, string systemProcessOperationId);
    }
}