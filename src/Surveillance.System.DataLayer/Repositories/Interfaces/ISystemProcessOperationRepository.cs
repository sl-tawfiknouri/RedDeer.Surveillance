using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRepository
    {
        Task Create(ISystemProcessOperation entity);
        Task Update(ISystemProcessOperation entity);
        Task<IReadOnlyCollection<ISystemProcessOperation>> GetDashboard();
    }
}