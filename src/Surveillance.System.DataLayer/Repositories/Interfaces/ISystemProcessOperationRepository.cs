using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRepository
    {
        Task Create(ISystemProcessOperation entity);
        Task Update(ISystemProcessOperation entity);
        Task<IReadOnlyCollection<ISystemProcessOperation>> GetDashboard();
    }
}