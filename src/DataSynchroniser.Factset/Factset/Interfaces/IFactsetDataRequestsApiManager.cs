using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace DataSynchroniser.Api.Factset.Factset.Interfaces
{
    public interface IFactsetDataRequestsApiManager
    {
        Task<FactsetSecurityResponseDto> Send(IReadOnlyCollection<MarketDataRequest> factsetRequests);
    }
}