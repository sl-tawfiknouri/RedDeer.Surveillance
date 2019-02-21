using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessRepository
    {
        Task Create(ISystemProcess entity);
        Task Update(ISystemProcess entity);
        Task<IReadOnlyCollection<ISystemProcess>> GetDashboard();
    }
}