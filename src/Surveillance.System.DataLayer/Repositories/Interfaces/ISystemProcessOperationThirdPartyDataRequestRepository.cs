using System.Threading.Tasks;
using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationThirdPartyDataRequestRepository
    {
        Task Create(ISystemProcessOperationThirdPartyDataRequest entity);
    }
}