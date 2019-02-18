using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace DataSynchroniser.Manager.Factset.Interfaces
{
    public interface IFactsetDataRequestsSenderManager
    {
        Task<FactsetSecurityResponseDto> Send(List<MarketDataRequest> factsetRequests);
    }
}