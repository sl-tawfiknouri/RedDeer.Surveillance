using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace DataSynchroniser.Manager.Factset.Interfaces
{
    public interface IFactsetDataRequestsSenderManager
    {
        Task<FactsetSecurityResponseDto> Send(List<MarketDataRequestDataSource> factsetRequests);
    }
}