using System.Threading.Tasks;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationThirdPartyDataRequestRepository
    {
        Task Create(ISystemProcessOperationThirdPartyDataRequest entity);
    }
}