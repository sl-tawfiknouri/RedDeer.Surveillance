using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace DataSynchroniser.Manager.Factset.Interfaces
{
    public interface IFactsetDataRequestsStorageManager
    {
        Task Store(FactsetSecurityResponseDto dto);
    }
}