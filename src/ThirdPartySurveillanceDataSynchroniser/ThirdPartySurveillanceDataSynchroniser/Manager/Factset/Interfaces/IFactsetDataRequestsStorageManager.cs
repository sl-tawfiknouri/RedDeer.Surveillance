using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Factset.Interfaces
{
    public interface IFactsetDataRequestsStorageManager
    {
        Task Store(FactsetSecurityResponseDto dto);
    }
}