namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationRepository
    {
        Task Create(ISystemProcessOperation entity);

        Task<IReadOnlyCollection<ISystemProcessOperation>> GetDashboard();

        Task Update(ISystemProcessOperation entity);
    }
}