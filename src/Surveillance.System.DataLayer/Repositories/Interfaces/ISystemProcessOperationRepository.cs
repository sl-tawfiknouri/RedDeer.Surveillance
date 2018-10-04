using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRepository
    {
        void Create(ISystemProcessOperation entity);
        void Update(ISystemProcessOperation entity);
    }
}