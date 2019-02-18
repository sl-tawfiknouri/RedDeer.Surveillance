using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace DataSynchroniser.Api.Factset.Factset.Interfaces
{
    public interface IFactsetDataRequestsStorageManager
    {
        Task Store(FactsetSecurityResponseDto dto);
    }
}