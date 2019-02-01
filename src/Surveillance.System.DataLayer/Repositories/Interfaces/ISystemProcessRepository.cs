using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessRepository
    {
        Task Create(ISystemProcess entity);
        Task Update(ISystemProcess entity);
        Task<IReadOnlyCollection<ISystemProcess>> GetDashboard();
    }
}