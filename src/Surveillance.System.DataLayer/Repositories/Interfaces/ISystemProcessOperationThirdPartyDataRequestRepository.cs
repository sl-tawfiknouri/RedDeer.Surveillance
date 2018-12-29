using System.Threading.Tasks;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationThirdPartyDataRequestRepository
    {
        Task Create(ISystemProcessOperationThirdPartyDataRequest entity);
    }
}