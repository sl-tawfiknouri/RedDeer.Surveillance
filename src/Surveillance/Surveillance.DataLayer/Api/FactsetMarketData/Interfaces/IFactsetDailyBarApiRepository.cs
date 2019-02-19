using System.Threading;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace Surveillance.DataLayer.Api.FactsetMarketData.Interfaces
{
    public interface IFactsetDailyBarApiRepository
    {
        Task<FactsetSecurityResponseDto> Get(FactsetSecurityDailyRequest request);
        Task<FactsetSecurityResponseDto> GetWithTransientFaultHandling(FactsetSecurityDailyRequest request);
        Task<bool> HeartBeating(CancellationToken token);
    }
}