namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessRepository
    {
        Task Create(ISystemProcess entity);

        Task<IReadOnlyCollection<ISystemProcess>> ExpiredProcessesWithQueues();

        Task<IReadOnlyCollection<ISystemProcess>> GetDashboard();

        Task Update(ISystemProcess entity);
    }
}