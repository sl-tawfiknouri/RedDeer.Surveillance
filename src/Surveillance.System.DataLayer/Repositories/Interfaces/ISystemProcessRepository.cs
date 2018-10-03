using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessRepository
    {
        void Create(ISystemProcess entity);
        void Update(ISystemProcess entity);
    }
}