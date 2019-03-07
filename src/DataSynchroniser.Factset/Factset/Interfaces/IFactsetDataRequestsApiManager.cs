using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using SharedKernel.Contracts.Markets;

namespace DataSynchroniser.Api.Factset.Factset.Interfaces
{
    public interface IFactsetDataRequestsApiManager
    {
        Task<FactsetSecurityResponseDto> Send(IReadOnlyCollection<MarketDataRequest> factsetRequests);
    }
}