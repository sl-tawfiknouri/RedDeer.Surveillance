namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationDistributeRuleRepository
    {
        Task Create(ISystemProcessOperationDistributeRule entity);

        Task<IReadOnlyCollection<ISystemProcessOperationDistributeRule>> GetDashboard();

        Task Update(ISystemProcessOperationDistributeRule entity);
    }
}