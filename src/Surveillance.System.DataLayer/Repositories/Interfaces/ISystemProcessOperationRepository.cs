using Surveillance.System.Auditing.Processes;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRepository
    {
        void Create(SystemProcessOperation entity);
    }
}