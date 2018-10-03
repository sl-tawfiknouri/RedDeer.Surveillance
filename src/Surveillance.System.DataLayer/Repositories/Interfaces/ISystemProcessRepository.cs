using Surveillance.System.Auditing.Processes;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessRepository
    {
        void Create(SystemProcess entity);
        void Update(SystemProcess entity);
    }
}