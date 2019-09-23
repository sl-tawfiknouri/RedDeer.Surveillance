namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    using System.Threading.Tasks;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationThirdPartyDataRequestRepository
    {
        Task Create(ISystemProcessOperationThirdPartyDataRequest entity);
    }
}